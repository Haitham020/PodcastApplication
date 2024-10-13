using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class SavePlaylistsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public SavePlaylistsController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> ToggleSavePLaylist(int playlistId, bool isSaving)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (isSaving)
            {
                var isSaved = await _db.SavedPlaylists
                    .AnyAsync(x => x.PlaylistId == playlistId && x.UserId == userId);

                if (!isSaved)
                {
                    SavedPlaylist savedPlaylist = new SavedPlaylist
                    {
                        UserId = userId,
                        PlaylistId = playlistId
                    };
                    await _db.SavedPlaylists.AddAsync(savedPlaylist);
                    await _db.SaveChangesAsync();
                }

            }
            else
            {
                var alreadySaved = await _db.SavedPlaylists
                    .FirstOrDefaultAsync(x => x.PlaylistId == playlistId && x.UserId == userId);
                if (alreadySaved != null)
                {
                    _db.SavedPlaylists.Remove(alreadySaved);
                    await _db.SaveChangesAsync();
                }

            }

            return Json(new { success = true });
        }
    }
}
