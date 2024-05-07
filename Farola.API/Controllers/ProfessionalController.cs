using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Queries;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetSpecializations")]
        public async Task<IActionResult> GetSpecializations()
        {
            return Ok(_context.Specializations.AsEnumerable());
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
                .Select(g => new SpecStat { Name = g.Key, Count = g.Count()})
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
