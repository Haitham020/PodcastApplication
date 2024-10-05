using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class PodcastsController : Controller
    {
        private AppDbContext db;
        public PodcastsController(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<IActionResult> AllPodcasts()
        {
            var allPodcasts = await db.Podcasts.Include(u => u.Creator)
                .Include(r => r.Ratings)
                .Include(s => s.Subscriptions)
                .ToListAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userPlaylists = db.Playlists
                .Where(p => p.UserId == userId)
                .ToList();

            ViewBag.UserPlaylists = userPlaylists;

            return View(allPodcasts);
        }
        [HttpGet]
        public async Task<IActionResult> PodcastDetails(Guid? id)
        {
            var podcast = await db.Podcasts
                .Include(e => e.Episodes)!
                    .ThenInclude(x => x.Comments)
                .Include(e => e.Episodes)!
                    .ThenInclude(i => i.EpisodeLikes)
                .Include(e => e.Episodes)!
                    .ThenInclude(a => a.EpisodeListeners)
                .Include(c => c.Category)
                .Include(u => u.Creator)
                .FirstOrDefaultAsync(p => p.PodcastId == id);

            if(podcast == null)
            {
                return NotFound();
            }

            return View(podcast);
        }

    }
}
