using Farola.API.Infrastructure.Commands;
using Farola.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Farola.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatementController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FarolaContext _context;

        public StatementController(IMediator mediator, FarolaContext context)
        {
            _mediator = mediator;
            _context = context; 
        }

        [HttpGet("GetStatementsById/{proId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetStatementsById(int proId)
        {
            return Ok(_context.Statements.Where(s => s.ProfessionalId == proId).AsEnumerable());
        }

        [HttpPost("Send")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> SendStatement([FromBody] SendStatementCommand sendStatementCommand)
        {
            var statement = await _mediator.Send(sendStatementCommand);
            return statement is not null ? Created(nameof(SendStatement), statement) : BadRequest("Не удалось создать заявку");
        }

        [HttpGet("IsExistActive/{clientId}/{proId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> IsExistActive(int clientId, int proId)
        {
            var a = await _context.Statements.AnyAsync(s => s.ClientId == clientId && s.ProfessionalId == proId && s.StatusId != 3);
            return Ok(a);
        }
    }
}
