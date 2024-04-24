using Farola.API.Infrastructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Farola.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : Controller
    {
        private readonly IMediator _mediator;
        public ApplicantController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="registrationUserCommand">Данные пользователя</param>
        /// <returns>Токен</returns>
        [HttpPost("SignUp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] RegistrationApplicantCommand registrationUserCommand)
        {
            var user = await _mediator.Send(registrationUserCommand);
            return user is not null ? Created(nameof(SignUp), user) : BadRequest(ModelState);
        }
    }
}
