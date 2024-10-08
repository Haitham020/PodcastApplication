using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class EpisodeLikesController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public EpisodeLikesController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(Guid episodeId, bool isLiking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
            }
            

            if (isLiking)
            {
                var isLikeExisted = await _db.EpisodeLikes
                .AnyAsync(x => x.EpisodeId == episodeId && x.UserId == userId);

                if (!isLikeExisted)
                {
                    EpisodeLike like = new EpisodeLike
                    {
                        UserId = userId,
                        EpisodeId = episodeId,
                    };
                    await _db.EpisodeLikes.AddAsync(like);
                    await _db.SaveChangesAsync();
                }

            }
            else
            {
                var liked = await _db.EpisodeLikes
                    .FirstOrDefaultAsync(x => x.EpisodeId == episodeId && x.UserId == userId);
                if(liked != null)
                {
                    _db.EpisodeLikes.Remove(liked);
                    await _db.SaveChangesAsync();
                }
                
            }

            var likeCount = await _db.EpisodeLikes
                .CountAsync(x => x.EpisodeId == episodeId);

            return Json(new { success = true, likeCount });
                    
        }
    }
}
