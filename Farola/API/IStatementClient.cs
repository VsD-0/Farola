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

        [Get("/Statement/GetStatementsByClientId/{clientId}")]
        Task<IEnumerable<StatementsViewModel>> GetStatementsByClientId(int clientId);

        [Post("/Statement/Send")]
        Task<Statement> SendStatement(SendStatement request);

        [Get("/Statement/GetStatuses")]
        Task<IEnumerable<StatementStatus>> GetStatuses();

        [Post("/Statement/UpdateStatus/{statementId}/{newStatus}")]
        Task UpdateStatus(int statementId, int newStatus);

        [Post("/Statement/SaveGrade/{statementId}/{grade}/{role}")]
        Task SaveGrade(int statementId, float? grade, string role);

        [Post("/Statement/SaveComment/{statementId}/{comment}/{role}")]
        Task SaveComment(int statementId, string comment, string role);

        [Get("/Statement/IsExistActive/{id}/{proId}")]
        Task<bool> IsExistActive(int id, int proId);

        [Get("/Statement/IsExistGrade/{statementId}")]
        Task<bool> IsExistGrade(int statementId);
    }
}
