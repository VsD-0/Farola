using Farola.Database.Models;
using Farola.Domain.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IUserClient
    {
        [Post("/user/signin")]
        Task<string> SignIn([Body] AuthModel auth);

        [Get("/user/{id}")]
        Task<User> GetUserById(int id);
    }
}
