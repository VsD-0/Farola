using Farola.Database.Models;
using Farola.Domain.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IStatementClient
    {
        [Get("/Statement/GetStatementsById/{proId}")]
        Task<IEnumerable<StatementsViewModel>> GetStatementsById(int proId);

        [Post("/Statement/Send")]
        Task<Statement> SendStatement(SendStatement request);

        [Get("/Statement/GetStatuses")]
        Task<IEnumerable<StatementStatus>> GetStatuses();

        [Post("/Statement/UpdateStatus/{statementId}/{newStatus}")]
        Task UpdateStatus(int statementId, int newStatus);

        [Post("/Statement/SaveGrade/{statementId}/{grade}")]
        Task SaveGrade(int statementId, float? grade);

        [Post("/Statement/SaveComment/{statementId}/{comment}")]
        Task SaveComment(int statementId, string comment);

        [Get("/Statement/IsExistActive/{id}/{proId}")]
        Task<bool> IsExistActive(int id, int proId);
    }
}
