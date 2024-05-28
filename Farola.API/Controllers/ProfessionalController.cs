using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Queries;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading;


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
                .Select(x => x.Review)
                .AsEnumerable());
        }

        [HttpGet("GetReviewBestPro")]
        public async Task<IActionResult> GetReviewBestPro(int pageNumber, int pageSize)
        {
            

            return Ok();
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
