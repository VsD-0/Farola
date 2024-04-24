using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    /// Обработчик авторизации
    /// </summary>
    public class RegistrationApplicantHandler : IRequestHandler<RegistrationApplicantCommand, UserDTO>
    {
        private readonly FarolaContext _context;

        public RegistrationApplicantHandler(FarolaContext context)
        {
            _context = context;
        }

        public async Task<UserDTO> Handle(RegistrationApplicantCommand request, CancellationToken cancellationToken)
        {
            User newUser = new()
            {
                Surname = request.Surname,
                Name = request.Name,
                PhoneNumber = request.Phone_number,
                Email = request.Email,
                Password = request.Password,
                Role = 2,
                Area = request.Area,
                Photo = request.Photo
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = newUser.Id,
                Surname = newUser.Surname,
                Name = newUser.Name,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Area = newUser.Area,
                Photo = newUser.Photo
            };
        }
    }
}
