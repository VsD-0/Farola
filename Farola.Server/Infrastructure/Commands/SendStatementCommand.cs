using Farola.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Farola.Server.Infrastructure.Commands
{
    /// <summary>
    /// Команда регистрации заявления
    /// </summary>
    public class SendStatementCommand : IRequest<Statement>
    {
        /// <summary>
        /// Номер специалиста
        /// </summary>
        /// <example>1</example>
        [FromBody]
        [BindRequired]
        public int ProfessionalId { get; set; }
        /// <summary>
        /// Номер клиента
        /// </summary>
        /// <example>2</example>
        [FromBody]
        [BindRequired]
        public int ClientId { get; set; }
    }

    /// <summary>
    /// Обработчик регистрации заявления
    /// </summary>
    public class SendStatementHandler(FarolaContext context) : IRequestHandler<SendStatementCommand, Statement>
    {
        private readonly FarolaContext _context = context;

        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Заявление</returns>
        public async Task<Statement> Handle(SendStatementCommand request, CancellationToken cancellationToken)
        {
            Statement newStatement = new()
            {
                ClientId = request.ClientId,
                ProfessionalId = request.ProfessionalId,
                StatusId = 1
            };

            await _context.Statements.AddAsync(newStatement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newStatement;
        }
    }
}
