using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using PodcastApplication.Models.ViewModels;
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
                .AsNoTrackingWithIdentityResolution()
                .Where(p => p.IsPublic && p.IsActive)
                .ToListAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userPlaylists = db.Playlists
                .Where(p => p.UserId == userId && p.IsActive)
                .ToList();

            ViewBag.UserPlaylists = userPlaylists;

            return View(allPodcasts);
        }
        [HttpGet]
        public async Task<IActionResult> PodcastDetails(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var podcast = await db.Podcasts
                .Include(e => e.Episodes)!
                    .ThenInclude(x => x.Comments)
                .Include(e => e.Episodes)!
                    .ThenInclude(i => i.EpisodeLikes)
                .Include(e => e.Episodes)!
                    .ThenInclude(a => a.EpisodeListeners)
                .Include(c => c.Category)
                .Include(u => u.Creator)
                .Include(s => s.Subscriptions)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(p => p.PodcastId == id);

            if (podcast == null)
            {
                return NotFound();
            }

            var episodes =  podcast.Episodes!
                .Where(x => x.PodcastId == id && x.IsActive && x.IsPublic)
                .OrderBy(x => x.CreatedAt)
                .ToList();

            int episodeNum = 1;
            foreach (var epis in episodes)
            {
                epis.EpisodeNumber = episodeNum;
                episodeNum++;
            }

            var userRating = await db.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PodcastId == id);

            int userRatingValue = userRating != null ? userRating.RatingValue : 0;


            PodcastDetailViewModel model = new PodcastDetailViewModel
            {
                Podcast = podcast,
                Episodes = episodes,
                UserRatingValue = userRatingValue
            };
            var userPlaylists = db.Playlists
                .Where(p => p.UserId == userId && p.IsActive)
                .ToList();

            ViewBag.UserPlaylists = userPlaylists;

            return View(model);
        }

    }
}
