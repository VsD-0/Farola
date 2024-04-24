using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IUserClient
    {
        [Post("/user/signin")]
        Task SignIn([Body] AuthModel auth);
    }
}
