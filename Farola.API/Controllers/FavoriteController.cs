using Farola.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Farola.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly FarolaContext _context;

        public FavoriteController(IMediator mediator, FarolaContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("GetFavorites/{clientId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetFavorites(int clientId) => Ok(await _context.Favorites
                .Where(f => f.ClientId == clientId)
                .Join(_context.Users.Where(u => u.RoleId == 1),
                f => f.ProfessionalId,
                p => p.Id,
                (f, p) => new { Favorite = f, Professional = p })
                .Select(x => x.Professional).ToListAsync());

        [HttpPost("AddFavorite/{proid}/{clientid}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> AddFavorite(int proid, int clientid)
        {
            await _context.Favorites.AddAsync(new Favorite { ProfessionalId = proid, ClientId = clientid });
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("DeleteFavorite/{proid}/{clientid}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteFavorite(int proid, int clientid)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.ClientId == clientid && f.ProfessionalId == proid);
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("IsFavorite/{proid}/{clientid}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> IsFavorite(int proid, int clientid) =>
            Ok(await _context.Favorites.AnyAsync(f => f.ClientId == clientid && f.ProfessionalId == proid));
    }
}
