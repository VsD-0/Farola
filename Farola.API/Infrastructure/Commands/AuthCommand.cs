using Farola.API.Infrastructure.Extensions;
using Farola.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
