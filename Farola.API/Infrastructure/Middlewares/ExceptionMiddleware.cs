using Farola.API.Infrastructure.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace Farola.API.Infrastructure.Middlewares
{
    /// <summary>
    /// Промежуточное ПО для обработки исключений.
    /// </summary>
    /// <remarks>
    /// Инициализирует новый экземпляр класса <see cref="ExceptionMiddleware"/>.
    /// </remarks>
    /// <param name="next">Следующий делегат запроса.</param>
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Выполняет обработку исключений.
        /// </summary>
        /// <param name="context">Контекст HTTP-запроса.</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateProblemDetails(context, statusCode: (int)HttpStatusCode.InternalServerError);

                problemDetails.Title = "Ошибка валидации";
                problemDetails.Detail = ex.Message;
                problemDetails.Status = 400;
                problemDetails.Extensions["errors"] = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();

                context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception ex)
            {
                var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateProblemDetails(context, statusCode: (int)HttpStatusCode.InternalServerError);

                problemDetails.Title = "Произошла ошибка";
                problemDetails.Detail = ex.Message;

                context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
