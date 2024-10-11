using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class FavoritesController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public FavoritesController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(Guid episodeId, bool addingFavorite)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
            }
            if (addingFavorite)
            {
                var isFavoriteExisted = await _db.Favorites
                .AnyAsync(x => x.EpisodeId == episodeId && x.UserId == userId);

                if (!isFavoriteExisted)
                {
                    Favorite favorite = new Favorite
                    {
                        UserId = userId,
                        EpisodeId = episodeId,
                    };
                    await _db.Favorites.AddAsync(favorite);
                    await _db.SaveChangesAsync();
                }

            }
            else
            {
                var favoriteExist = await _db.Favorites
                    .FirstOrDefaultAsync(x => x.EpisodeId == episodeId && x.UserId == userId);
                if (favoriteExist != null)
                {
                    _db.Favorites.Remove(favoriteExist);
                    await _db.SaveChangesAsync();
                }

            }

            return Json(new { success = true});

        }
        public async Task<IActionResult> MyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var favoriteEpisodes = await _db.Favorites
                .Where(u => u.UserId == userId)
                .Include(e => e.Episode)
                .Select(e => e.Episode)
                .ToListAsync();

            return View(favoriteEpisodes);
        }
    }
}
