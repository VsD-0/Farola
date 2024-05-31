using Farola.Database.Models;
using Farola.Domain.Models;
using Farola.ViewModels;
using Refit;

namespace Farola.API
{
    public interface IProfessionalClient
    {
        [Get("/Professional/GetProfessionals")]
        Task<PaginatedResult<UserDTO>> GetProfessionals(int pageNumber, int pageSize, string? profession, string? specialization);
        [Get("/Professional/GetReviewBestPro")]
        Task<PaginatedResult<ReviewViewModel>> GetReviewBestPro(int pageNumber, int pageSize);

        [Get("/Professional/GetProfessional/{id}")]
        Task<UserDTO> GetProfessional(int id);

        [Get("/Professional/GetSpecializations")]
        Task<IEnumerable<Specialization>> GetSpecializations();

        [Get("/Professional/GetReviewsByUser/{userid}")]
        Task<IEnumerable<ReviewViewModel?>> GetReviewsByUser(int userId);

        [Get("/Professional/GetSpecStats")]
        Task<IEnumerable<SpecStat>> GetSpecStats();

        [Post("/Professional/SignUp")]
        Task<UserDTO> SignUp([Body] RegProfessional user);
    }
}
