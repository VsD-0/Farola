using Farola.Database.Models;
using Refit;

namespace Farola.API
{
    public interface IFavoriteClient
    {
        [Get("/Favorite/GetFavorites/{clientid}")]
        Task<List<User>> GetFavorites(int clientId);

        [Post("/Favorite/AddFavorite/{proid}/{clientid}")]
        Task AddFavorite(int proid, int clientid);

        [Post("/Favorite/DeleteFavorite/{proid}/{clientid}")]
        Task DeleteFavorite(int proid, int clientid);

        [Get("/Favorite/IsFavorite/{proid}/{clientid}")]
        Task<bool> IsFavorite(int proid, int clientid);
    }
}
