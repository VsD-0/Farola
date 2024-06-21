using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Cryptography;

namespace Farola.API.Infrastructure.Commands
{
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    public class RegistrationApplicantCommand : IRequest<UserDTO>
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        /// <example>Иванов</example>
        [FromBody]
        [BindRequired]
        public string? Surname { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        /// <example>Иван</example>
        [FromBody]
        [BindRequired]
        public string? Name { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        /// <example>Иванович</example>
        [FromBody]
        [BindRequired]
        public string? Patronymic { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        /// <example>89271584836</example>
        [FromBody]
        [BindRequired]
        public string? Phone_number { get; set; }
        /// <summary>
        /// Логин
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
        /// Место работы
        /// </summary>
        /// <example>Уфа</example>
        [FromBody]
        [BindRequired]
        public string? Area { get; set; }
        /// <summary>
        /// Фото
        /// </summary>
        /// <example>photo.jpg</example>
        [FromBody]
        [BindRequired]
        public string? Photo { get; set; }
    }

    /// <summary>
    /// Обработчик регистрации
    /// </summary>
    public class RegistrationApplicantHandler(FarolaContext context) : IRequestHandler<RegistrationApplicantCommand, UserDTO>
    {
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Пользователь</returns>
        public async Task<UserDTO> Handle(RegistrationApplicantCommand request, CancellationToken cancellationToken)
        {
            User newUser = new()
            {
                Surname = request.Surname,
                Name = request.Name,
                Patronymic = request.Patronymic,
                PhoneNumber = request.Phone_number,
                Email = request.Email,
                RoleId = 2,
                Area = request.Area,
                Photo = request.Photo
            };
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);
            newUser.Password = passwordHash;

            var refreshToken = GenerateRefreshToken();
            newUser.RefreshToken = refreshToken;

            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var refreshTokenEntry = new RefreshToken
            {
                Userid = newUser.Id,
                Createdat = DateTime.UtcNow,
                Expiresat = DateTime.UtcNow.AddDays(30),
                Token = refreshToken
            };
            await _context.RefreshTokens.AddAsync(refreshTokenEntry, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new UserDTO
            {
                Id = newUser.Id,
                RoleId = newUser.RoleId,
                Surname = newUser.Surname,
                Patronymic = newUser.Patronymic,
                Name = newUser.Name,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Area = newUser.Area,
                Photo = newUser.Photo,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}
