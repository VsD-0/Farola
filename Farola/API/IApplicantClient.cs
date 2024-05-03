using Farola.Database.Models;
using Farola.Domain.Models;
using Refit;

namespace Farola.API
{
    public interface IApplicantClient
    {
        [Get("/Applicant/SignUp")]
        Task<UserDTO> SignUp([Body] User user);
    }
}
