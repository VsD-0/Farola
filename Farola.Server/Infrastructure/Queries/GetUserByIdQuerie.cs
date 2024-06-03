using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Farola.Server.Infrastructure.Queries
{
    /// <summary>
    /// Получения пользователя по его Id
    /// </summary>
    public class GetUserByIdQuerie : IRequest<User>
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
    /// <remarks>
    /// Подключение базы данных
    /// </remarks>
    /// <param name="context">Контекст базы данных.</param>
    public class GetUserByIdHandler(FarolaContext context) : IRequestHandler<GetUserByIdQuerie, User?>
    {
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Найденный пользователь или null, если пользователь не найден.</returns>
        public async Task<User?> Handle(GetUserByIdQuerie request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            return user;
        }
    }
}
