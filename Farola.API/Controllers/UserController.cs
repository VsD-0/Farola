using Farola.API.Infrastructure.Commands;
using Farola.API.Infrastructure.Exceptions;
using Farola.API.Infrastructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farola.API.Controllers
{
    /// <summary>
    /// Контроллер пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        protected readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр интерфейса IMediator.</param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получение пользователя по id
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns>Пользователь</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _mediator.Send(new GetUserByIdQuerie { Id = id });
            return user is not null ? Ok(user) : NotFound(null);
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="authCommand">Данные пользователя (электронная почта, пароль)</param>
        /// <returns>Токен</returns>
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] AuthCommand authCommand)
        {
            var token = await _mediator.Send(authCommand);
            return token is not null ? Ok(token) : BadRequest("Пользователь не найден");
        }
    }
}
