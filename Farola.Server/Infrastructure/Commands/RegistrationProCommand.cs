using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Farola.Server.Infrastructure.Commands
{
    /// <summary>
    /// Авторизация специалиста
    /// </summary>
    public class RegistrationProCommand : IRequest<UserDTO>
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        /// <example>Иванов</example>
        [FromBody]
        [BindRequired]
        public string Surname { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        /// <example>Иван</example>
        [FromBody]
        [BindRequired]
        public string Name { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        /// <example>89271584836</example>
        [FromBody]
        [BindRequired]
        public string Phone_number { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        /// <example>ivan.ivanov@gmail.com</example>
        [FromBody]
        [BindRequired]
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        /// <example>Ivan123#</example>
        [FromBody]
        [BindRequired]
        public string Password { get; set; }
        /// <summary>
        /// Место работы
        /// </summary>
        /// <example>Уфа</example>
        [FromBody]
        [BindRequired]
        public string? Area { get; set; }
        /// <summary>
        /// Подробная информация
        /// </summary>
        /// <example>*Текст подробной информации о специалисте*</example>
        [FromBody]
        [BindRequired]
        public string? Information { get; set; }
        /// <summary>
        /// Специализация
        /// </summary>
        /// <example>1</example>
        [FromBody]
        [BindRequired]
        public int Specialization { get; set; }
        /// <summary>
        /// Профессия
        /// </summary>
        /// <example>Сварщик</example>
        [FromBody]
        [BindRequired]
        public string Profession { get; set; }
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
    /// <remarks>
    /// Инициализирует новый экземпляр класса <see cref="RegistrationProHandler"/>.
    /// </remarks>
    /// <param name="context">Контекст базы данных.</param>
    public class RegistrationProHandler(FarolaContext context) : IRequestHandler<RegistrationProCommand, UserDTO>
    {
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сгенерированный JWT-токен</returns>
        public async Task<UserDTO> Handle(RegistrationProCommand request, CancellationToken cancellationToken)
        {
            User newUser = new()
            {
                Surname = request.Surname,
                Name = request.Name,
                PhoneNumber = request.Phone_number,
                Email = request.Email,
                Password = request.Password,
                RoleId = 1,
                Area = request.Area,
                Photo = request.Photo,
                Information = request.Information,
                Profession = request.Profession,
                SpecializationId = request.Specialization
            };

            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new UserDTO
            {
                Id = newUser.Id,
                RoleId = newUser.RoleId,
                Surname = newUser.Surname,
                Name = newUser.Name,
                Patronymic = newUser.Patronymic,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Area = newUser.Area,
                Photo = newUser.Photo,
                Information = request.Information,
                Specialization = (await _context.Specializations.SingleOrDefaultAsync(s => s.Id == newUser.SpecializationId, cancellationToken))?.Name,
                Profession = request.Profession
            };
        }
    }
}
