using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Farola.API.Infrastructure.Queries
{
    /// <summary>
    /// Команда получения списка профессионалов на странице
    /// </summary>
    public class GetProfessionalsQuerie : IRequest<PaginatedResult<UserDTO>>
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; set; }

        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        /// <example>5</example>
        public int PageSize { get; set; }

        /// <summary>
        ///Профессия
        /// </summary>
        /// <example>Веб разработчик</example>
        public string? Profession { get; set; }

        /// <summary>
        /// Идентификатор специализации
        /// </summary>
        /// <example>1</example>
        public string? Specialization { get; set; }
    }

    /// <summary>
    /// Обработчик команды получения профессионалов с пагинацией.
    /// </summary>
    /// <remarks>
    /// Подключение базы данных
    /// </remarks>
    /// <param name="context">Контекст базы данных</param>
    public class GetProfessionalsHandler(FarolaContext context) : IRequestHandler<GetProfessionalsQuerie, PaginatedResult<UserDTO>>
    {
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        public async Task<PaginatedResult<UserDTO>> Handle(GetProfessionalsQuerie request, CancellationToken cancellationToken)
        {
            _ = int.TryParse(request.Specialization, out int specId);
            var pros = await _context.Users
                .Where(u => u.RoleId == 1 &&
                (request.Profession == null || (u.Profession != null && u.Profession.ToLower().Contains(request.Profession.ToLower()))) &&
                ((request.Specialization == null || specId == 0) ||
                u.SpecializationId == specId))
                .Select(p => new UserDTO
                {
                    Id = p.Id,
                    RoleId = p.RoleId,
                    Surname = p.Surname,
                    Name = p.Name,
                    Area = p.Area,
                    Email = p.Email,
                    Information = p.Information,
                    PhoneNumber = p.PhoneNumber,
                    Photo = p.Photo,
                    Profession = p.Profession,
                    Specialization = _context.Specializations
                        .SingleOrDefault(s => s.Id == p.SpecializationId).Name
                })
                .ToListAsync(cancellationToken);

            var skip = (request.PageNumber - 1) * request.PageSize;
            var result = new List<UserDTO>(pros.Skip(skip).Take(request.PageSize));

            var pagination = new Pagination<UserDTO>
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = pros.Count
            };

            return new PaginatedResult<UserDTO>
            {
                Pagination = pagination,
                Items = result
            };
        }
    }
}
