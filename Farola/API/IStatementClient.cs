using Farola.Database.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IStatementClient
    {
        [Get("/Statement/GetStatementsById/{proId}")]
        Task<IEnumerable<Statement>> GetStatementsById(int proId);

        [Post("/Statement/Send")]
        Task<Statement> SendStatement(SendStatement request);

        [Get("/Statement/IsExistActive/{id}/{proId}")]
        Task<bool> IsExistActive(int id, int proId);
    }
}
