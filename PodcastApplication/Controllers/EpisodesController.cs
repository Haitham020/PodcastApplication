using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using PodcastApplication.Models.ViewModels;

namespace PodcastApplication.Controllers
{
    public class EpisodesController : Controller
    {
        private AppDbContext db;
        public EpisodesController(AppDbContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> AllEpisodes()
        {
            var episodes = await db.Episodes
                .Include(c => c.Comments)
                .Include(i => i.EpisodeLikes)
                .Include(x => x.Podcast)
                .ThenInclude(x => x!.Creator)
                .OrderBy(e => e.CreatedAt)
                .Where(x => x.IsActive)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();

            foreach (var podcast in episodes.GroupBy(p => p.PodcastId))
            {

                int episodeNum = 1;
                foreach (var episode in podcast)
                {
                    episode.EpisodeNumber = episodeNum;
                    episodeNum++;
                }
            }
            var displayedEpisodes = episodes.Take(20);
            return View(displayedEpisodes);
        }

        [HttpGet]
        public async Task<IActionResult> EpisodeDetails(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await db.Episodes
                .Include(p => p.Podcast)
                    .ThenInclude(c => c!.Category)
                .Include(p => p.Podcast)
                    .ThenInclude(u => u!.Creator)
                .Include(x => x.EpisodeLikes)
                .FirstOrDefaultAsync(m => m.EpisodeId == id);

            if (episode == null)
            {
                return NotFound();
            }
            var podcastEpisodes = await db.Episodes
                .Where(p => p.PodcastId == episode.PodcastId && p.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            int episodeNum = 1;
            foreach (var epis in podcastEpisodes)
            {
                epis.EpisodeNumber = episodeNum;
                episodeNum++;
            }

            var relatedEpisodes = await db.Episodes.Include(x => x.Comments)
                .Include(x => x.EpisodeListeners)
                .Include(i => i.EpisodeLikes)
                .Include(p => p.Podcast)
                     .ThenInclude(u => u!.Creator)
                .Include(p => p.Podcast)
                     .ThenInclude(c => c!.Category)
                .Where(c => c.Podcast!.CategoryId == episode.Podcast!.CategoryId
                && c.EpisodeId != episode.EpisodeId)
                .Take(3)
                .ToListAsync();

            EpisodeDetailViewModel episodeDetail = new EpisodeDetailViewModel
            {
                Episode = episode,
                RelatedEpisodes = relatedEpisodes,
                EpisodeNumber = episode.EpisodeNumber
            };

            return View(episodeDetail);
        }
    }
}
