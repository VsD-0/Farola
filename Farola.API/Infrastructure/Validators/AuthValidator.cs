using Farola.API.Infrastructure.Commands;
using Farola.Database.Models;
using FluentValidation;

namespace Farola.API.Infrastructure.Validators
{
    /// <summary>
    /// Валидатор для команды авторизации
    /// </summary>
    public class AuthValidator : AbstractValidator<AuthCommand>
    {
        private readonly FarolaContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthValidator"/>
        /// </summary>
        public AuthValidator(FarolaContext context)
        {
            _context = context;

            // Проверка логина
            RuleFor(x => x.Email)
               .NotNull()
               .WithState(x => "1234")
               .Must(IsExistLogin)
               .WithMessage("Пользователя с таким именем не существует");

            // Проверка пароля
            RuleFor(x => x)
               .NotNull()
               .WithName("Password")
               .WithMessage("Это обязательное поле")
               .Must(IsValidPassword)
               .WithName("Password")
               .WithMessage("Неверный пароль");
        }

        private bool IsExistLogin(string? email) => _context.Users.Any(u => u.Email == email);
        private bool IsValidPassword(AuthCommand user) => _context.Users.Any(u => u.Email == user.Email && u.Password == user.Password);
    }
}
