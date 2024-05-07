using Farola.Database.Models;
using Farola.Domain.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IApplicantClient
    {
        [Post("/Applicant/SignUp")]
        Task<UserDTO> SignUp([Body] RegApplicant user);
    }
}
