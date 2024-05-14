using Farola.Database.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IStatementClient
    {
        [Post("/Statement/Send")]
        Task<Statement> SendStatement(SendStatement request);

        [Get("/Statement/IsExistActive/{id}/{proId}")]
        Task<bool> IsExistActive(int id, int proId);
    }
}
