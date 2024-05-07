using Farola.Database.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IStatementClient
    {
        [Post("/Statement/Send")]
        Task<Statement> SendStatement(SendStatement request);
    }
}
