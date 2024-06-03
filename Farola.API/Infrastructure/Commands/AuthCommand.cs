using Farola.API.Infrastructure.Extensions;
using Farola.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Farola.API.Infrastructure.Commands
{
    /// <summary>
    /// Авторизация
    /// </summary>
    public class AuthCommand : IRequest<string?>
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        /// <example>ivan.ivanov@gmail.com</example>
        [FromBody]
        [BindRequired]
        public string? Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        /// <example>Ivan123#</example>
        [FromBody]
        [BindRequired]
        public string? Password { get; set; }

        /// <summary>
        /// Токен Обновления
        /// </summary>
        [FromBody]
        [BindRequired]
        public string? RefreshToken { get; set; }
    }

    /// <summary>
    /// Обработчик авторизации
    /// </summary>
    public class AuthHandler(IConfiguration configuration, FarolaContext context) : IRequestHandler<AuthCommand, string?>
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <exception cref="ArgumentNullException">Отсутствуют настройки jwt</exception>
        /// <returns>Найденный пользователь или null, если пользователь не найден</returns>
        public async Task<string?> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return null;

            if (request.RefreshToken != null)
            {
                var refreshToken = await ValidateRefreshToken(request.RefreshToken);
                if (refreshToken != null)
                {
                    var newAccessToken = GenerateAccessToken(refreshToken.User);
                    var newRefreshToken = GenerateRefreshToken();

                    await UpdateRefreshToken(refreshToken.User.Id, newRefreshToken);

                    return newAccessToken;
                }
            }

            var jwtSettings = _configuration.GetSection("JwtSettings")?.Get<JwtSettings>() ?? throw new ArgumentNullException("jwtSettings", "Отсутствуют настройки jwt");
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings?.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(10),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<RefreshToken?> ValidateRefreshToken(string refreshToken) => await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken && r.Expiresat > DateTime.UtcNow);


        private string GenerateAccessToken(User user)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshToken = Convert.ToBase64String(randomNumber);

            return refreshToken;
        }

        private async Task UpdateRefreshToken(int userId, string newRefreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = newRefreshToken;
                await _context.SaveChangesAsync();
            }
        }
    }
}