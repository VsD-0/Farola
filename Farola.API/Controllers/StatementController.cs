using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farola.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatementController : Controller
    {
        private readonly IMediator _mediator;

        public StatementController(IMediator mediator) => _mediator = mediator;

        [HttpPost("Send")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> SendStatement([FromBody] SendStatementCommand sendStatementCommand)
        {
            var statement = await _mediator.Send(sendStatementCommand);
            return statement is not null ? Created(nameof(SendStatement), statement) : BadRequest("Не удалось создать заявку");
        }
    }
}
