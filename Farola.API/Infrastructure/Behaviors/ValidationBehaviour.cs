using FluentValidation;
using MediatR;

namespace Farola.API.Infrastructure.Behaviors
{
    /// <summary>
    /// Представляет поведение, выполняющее валидацию с использованием FluentValidation для запросов MediatR.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса, который будет проверяться.</typeparam>
    /// <typeparam name="TResponse">Тип ответа от следующего обработчика в конвейере.</typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ValidationBehaviour{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="validators">Коллекция валидаторов, которые будут применены.</param>
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        /// <summary>
        /// Обрабатывает поведение валидации, выполняя зарегистрированные валидаторы и выбрасывая исключение в случае неудачи валидации.
        /// </summary>
        /// <param name="request">Запрос, который будет проверен.</param>
        /// <param name="next">Делегат, представляющий следующий обработчик в конвейере.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Ответ от следующего обработчика в конвейере.</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            foreach (var validator in _validators)
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResult = await validator.ValidateAsync(context, cancellationToken);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);
            }
            return await next();
        }
    }
}
