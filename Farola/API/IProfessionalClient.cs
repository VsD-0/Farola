using Farola.Domain.Models;
using Refit;

namespace Farola.API
{
    public interface IProfessionalClient
    {
        [Get("/Professional/GetProfessionals")]
        Task<PaginatedResult<UserDTO>> GetProfessionals(int pageNumber, int pageSize);
    }
}
