using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Farola.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Farola.API.Infrastructure.Extensions;

namespace Farola.API.Infrastructure.Commands
{
    /// <summary>
    /// Авторизация
    /// </summary>
    public class AuthCommand : IRequest<string>
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
    public class AuthHandler : IRequestHandler<AuthCommand, string>
    {
        private readonly IConfiguration _configuration;
        private readonly FarolaContext _context;

        public AuthHandler(IConfiguration configuration, FarolaContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            // Получение пользователя по его логину
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();

            var tokenHandler = new JwtSecurityTokenHandler();

            var secretKey = Encoding.UTF8.GetBytes(jwtSettings?.SecretKey);

            // Описание токена
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            // Генерация токена
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
