using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Farola.Database.Models;
using Farola.Domain.Models;
using MediatR;

namespace Farola.API.Infrastructure.Commands
{
    public class SendStatementCommand : IRequest<Statement>
    {
        [FromBody]
        [BindRequired]
        public int Professional { get; set; }
        [FromBody]
        [BindRequired]
        public int Client { get; set; }
    }

    public class SendStatementHandler : IRequestHandler<SendStatementCommand, Statement>
    {
        private readonly FarolaContext _context;

        public SendStatementHandler(FarolaContext context)
        {
            _context = context;
        }

        public async Task<Statement> Handle(SendStatementCommand request, CancellationToken cancellationToken)
        {
            Statement newStatement = new()
            {
                Client = request.Client,
                Professional = request.Professional,
                StatusId = 1
            };

            await _context.Statements.AddAsync(newStatement);
            await _context.SaveChangesAsync();

            return newStatement;
        }
    }
}
