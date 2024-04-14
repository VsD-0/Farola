using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Xml.Linq;

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
        /// Список документов
        /// </summary>
        /// <example>5</example>
    }

    /// <summary>
    /// Обработчик команды получения профессионалов с пагинацией.
    /// </summary>
    public class GetProfessionalsHandler : IRequestHandler<GetProfessionalsQuerie, PaginatedResult<UserDTO>>
    {
        private readonly FarolaContext _context;

        public GetProfessionalsHandler(FarolaContext context) => _context = context;

        public async Task<PaginatedResult<UserDTO>> Handle(GetProfessionalsQuerie request, CancellationToken cancellationToken)
        {
            var pros = await _context.Users
                .Where(u => u.Role == 1)
                .Select(p => new UserDTO
                {
                    Id = p.Id,
                    Surname = p.Surname,
                    Name = p.Name,
                    Area = p.Area,
                    Email = p.Email,
                    Information = p.Information,
                    PhoneNumber = p.PhoneNumber,
                    Photo = p.Photo,
                    Profession = p.Profession,
                    Specialization = _context.Specializations.SingleOrDefault(s => s.Id == p.Specialization).Name
                })
                .ToListAsync();

            int totalItems = pros.Count;

            int skip = (request.PageNumber - 1) * request.PageSize;

            List<UserDTO> result = new(pros.Skip(skip).Take(request.PageSize));

            var pagination = new Pagination<UserDTO>
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems
            };

            return new PaginatedResult<UserDTO>
            {
                Pagination = pagination,
                Items = result
            };
        }
    }
}
