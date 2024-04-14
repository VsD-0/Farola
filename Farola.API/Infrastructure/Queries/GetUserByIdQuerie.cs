using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Farola.API.Infrastructure.Queries
{
    /// <summary>
    /// Получения пользователя по его Id
    /// </summary>
    public class GetUserByIdQuerie : IRequest<UserDTO>
    {
        /// <summary>
        /// Id документа
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
    }

    /// <summary>
    /// Обработчик команды получения пользователя по идентификатору.
    /// </summary>
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuerie, UserDTO?>
    {
        private readonly FarolaContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GetUserByIdHandler"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public GetUserByIdHandler(FarolaContext context) => _context = context;

        /// <summary>
        /// Обрабатывает запрос на получение пользователя по идентификатору.
        /// </summary>
        /// <param name="request">Запрос на получение пользователя.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
        public async Task<UserDTO?> Handle(GetUserByIdQuerie request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
            return new UserDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Area = user.Area,
                Information = user.Information,
                Profession = user.Profession,
                Photo = user.Photo,
                Specialization = _context.Specializations.SingleOrDefault(s => s.Id == user.Specialization).Name
            };
        }
    }
}
