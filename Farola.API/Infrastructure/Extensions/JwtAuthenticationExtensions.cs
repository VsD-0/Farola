using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Farola.API.Infrastructure.Extensions
{
    /// <summary>
    /// Настройки для JSON Web Token (JWT).
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Секретный ключ для подписи JWT.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Эмитент JWT.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Аудитория JWT.
        /// </summary>
        public string? Audience { get; set; }
    }

    /// <summary>
    /// Расширение для настройки аутентификации с использованием JWT токенов.
    /// </summary>
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Настройка аутентификацию с использованием JWT токенов.
        /// </summary>
        /// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>.</param>
        /// <param name="jwtSettings">Настройки JWT токена.</param>
        public static void ConfigureJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = true;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey ?? ""))
                };
            });

        }
    }
}
