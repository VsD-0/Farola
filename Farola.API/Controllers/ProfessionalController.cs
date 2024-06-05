using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Queries;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace Farola.API.Controllers
{
    /// <summary>
    /// Контроллер профессионалов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessionalController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FarolaContext _context;

        public ProfessionalController(IMediator mediator, FarolaContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("GetProfessionals")]
        public async Task<IActionResult> GetProfessionals(int pageNumber, int pageSize, string? profession, string? specialization)
        {
            return Ok(await _mediator.Send(new GetProfessionalsQuerie { PageNumber = pageNumber, PageSize = pageSize, Profession = profession, Specialization = specialization }));
        }

        [HttpGet("GetProfessional/{id}")] 
        public async Task<IActionResult> GetProfessional(int id)
        {
            return Ok(await _context.Users.Where(u => u.Id == id)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Area = u.Area,
                    Email = u.Email,
                    Information = u.Information,
                    PhoneNumber = u.PhoneNumber,
                    Photo = u.Photo,
                    Profession = u.Profession,
                    RoleId = u.RoleId,
                    Surname = u.Surname,
                    Patronymic = u.Patronymic,
                    Specialization = _context.Specializations.FirstOrDefault(s => s.Id == u.SpecializationId)!.Name
                })
                .FirstOrDefaultAsync());
        }

        [HttpGet("GetSpecializations")]
        public async Task<IActionResult> GetSpecializations()
        {
            return Ok(_context.Specializations.AsEnumerable());
        }

        [HttpGet("GetReviewsByUser/{userId}")]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            return Ok(_context.Reviews
                .Join(_context.Statements,
                r => r.StatementId,
                s => s.Id,
                (r, s) => new {Review = r, Statement = s})
                .Where(x => x.Statement.ProfessionalId == userId)
                .Select(x => new ReviewViewModel
                {
                    Client = _context.Users.FirstOrDefault(u => u.Id == x.Statement.ClientId),
                    Professional = _context.Users.FirstOrDefault(u => u.Id == x.Statement.ProfessionalId),
                    DateAdded = x.Review.DateAdded,
                    Grade = x.Review.Grade,
                    Text = x.Review.Text
                }).AsEnumerable());
        }

        [HttpGet("GetReviewBestPro")]
        public async Task<IActionResult> GetReviewBestPro(int pageNumber, int pageSize)
        {
            var reviews = _context.Statements
                .Join(_context.Reviews,
                s => s.Id,
                r => r.StatementId,
                (s, r) => new { Statement = s, Review = r })
                .Where(x => x.Review.Grade >= 4)
                .GroupBy(x => x.Statement.ProfessionalId)
                .Select(x => new ReviewViewModel
                {
                    Client = _context.Users.FirstOrDefault(u => u.Id == x.FirstOrDefault().Statement.ClientId),
                    Professional = _context.Users.FirstOrDefault(u => u.Id == x.FirstOrDefault().Statement.ProfessionalId),
                    AvgGrade = _context.Reviews
                    .Join(_context.Statements,
                        r => r.StatementId,
                        s => s.Id,
                        (r, s) => new { Statement = s, Review = r })
                    .Where(y => y.Statement.ProfessionalId == x.FirstOrDefault().Statement.ProfessionalId).Average(r => r.Review.Grade),
                    CountGrade = _context.Reviews
                    .Join(_context.Statements,
                        r => r.StatementId,
                        s => s.Id,
                        (r, s) => new { Statement = s, Review = r })
                    .Where(y => y.Statement.ProfessionalId == x.FirstOrDefault().Statement.ProfessionalId).Count(),
                    Grade = x.FirstOrDefault().Review.Grade,
                    Text = x.FirstOrDefault().Review.Text,
                    DateAdded = x.FirstOrDefault().Review.DateAdded
                })
                .OrderByDescending(x => x.Grade);

            var skip = (pageNumber - 1) * pageSize;
            var result = new List<ReviewViewModel>(reviews.Skip(skip).Take(pageSize));

            var pagination = new Pagination<ReviewViewModel>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = await reviews.CountAsync()
            };

            return Ok(new PaginatedResult<ReviewViewModel>
            {
                Pagination = pagination,
                Items = result
            });
        }


        [HttpGet("GetSpecStats")]
        public async Task<IActionResult> GetSpecStats()
        {
            return Ok(_context.Users
                .Join(_context.Specializations,
                    u => u.SpecializationId,
                    sp => sp.Id,
                    (u, sp) => new { User = u, Spec = sp })
                .Where(x => x.User.RoleId == 1)
                .GroupBy(x => x.Spec.Name)
                .Select(g => new SpecStat { Spec = g.Select(x => x.Spec).First(), Count = g.Count()})
                .OrderByDescending(x => x.Count)
                .AsEnumerable());
        }

        /// <summary>
        /// Регистрация профессионала
        /// </summary>
        /// <param name="registrationProCommand">Данные пользователя</param>
        /// <returns>Токен</returns>
        /// <response code="201">Успешное выполнение</response>
        /// <response code="400">Ошибка API</response>
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] RegistrationProCommand registrationProCommand)
        {
            var pro = await _mediator.Send(registrationProCommand);
            return pro is not null ? Created(nameof(SignUp), pro) : BadRequest("Ошибка авторизации");
        }
    }
}
