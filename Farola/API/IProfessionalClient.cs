using Farola.Database.Models;
using Farola.Domain.Models;
using Refit;

namespace Farola.API
{
    public interface IProfessionalClient
    {
        [Get("/Professional/GetProfessionals")]
        Task<PaginatedResult<UserDTO>> GetProfessionals(int pageNumber, int pageSize, string? profession, string? specialization);

        [Get("/Professional/GetSpecializations")]
        Task<IEnumerable<Specialization>> GetSpecializations();
    }
}
