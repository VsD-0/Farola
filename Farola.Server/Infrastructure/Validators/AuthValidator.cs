using Farola.Server.Infrastructure.Commands;
using Farola.Database.Models;
using FluentValidation;

namespace Farola.Server.Infrastructure.Validators
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
               .WithMessage("Это обязательное поле")
               .Must(IsExistLogin)
               .WithMessage("Пользователь не существует")
               .Custom((x, context) =>
               {
                   if (!_context.Users.Any(u => u.Email == x))
                   {
                       context.AddFailure("Email", "Пользователь не существует");
                   }
               }); ;

            // Проверка пароля
            RuleFor(x => x.Password)
                .NotNull()
               .WithName("Password")
               .WithMessage("Это обязательное поле");

            // Общая проверка
            RuleFor(x => x)
               .Must(IsValidPassword)
               .WithName("General")
               .WithMessage("Неверный пароль или адрес эл. почты");
        }

        private bool IsExistLogin(string? email) => _context.Users.Any(u => u.Email == email);
        private bool IsValidPassword(AuthCommand user) => _context.Users.Any(u => u.Email == user.Email && u.Password == user.Password);
    }
}
