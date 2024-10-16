using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using PodcastApplication.Models.ViewModels;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    [Authorize(Roles = "Creator")]
    public class CreatorsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public CreatorsController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> CreatorProfile()
        {
            var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (creatorId == null)
            {
                return Unauthorized();
            }

            var creator = await _userManager.FindByIdAsync(creatorId);

            if (creator == null)
            {
                return NotFound();
            }
            return View(creator);
        }
        [HttpGet]
        public async Task<IActionResult> StagingPodcast(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var podcast = await _db.Podcasts
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

            var episodes = podcast.Episodes!
                .Where(x => x.PodcastId == id)
                .OrderBy(x => x.CreatedAt)
                .ToList();

            int episodeNum = 1;
            foreach (var epis in episodes)
            {
                epis.EpisodeNumber = episodeNum;
                episodeNum++;
            }


            PodcastDetailViewModel model = new PodcastDetailViewModel
            {
                Podcast = podcast,
                Episodes = episodes,
            };

            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> TogglePublishPodcast(Guid id)
        {
            var podcast = await _db.Podcasts
        .Include(p => p.Episodes)
        .FirstOrDefaultAsync(p => p.PodcastId == id);

           
            if (podcast == null)
            {
                return Json(new { success = false, message = "Podcast not found." });
            }

            if (podcast!.IsActive)
            {
                podcast.IsActive = false;
                podcast.IsDeleted = true;
                foreach (var episode in podcast.Episodes!)
                {
                    episode.IsActive = false;
                    episode.IsDeleted = true;
                }
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Podcast unpublished successfully!" });

            }
            else
            {
                if (podcast.Episodes!.Any() && podcast.Episodes != null)
                {
                    podcast.IsActive = true;
                    podcast.IsDeleted = false;
                    foreach (var episode in podcast.Episodes)
                    {
                        episode.IsActive = true;
                        episode.IsDeleted = false;
                    }

                    await _db.SaveChangesAsync();
                    return Json(new { success = true, message = "Podcast published successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cannot publish an Podcast without episode(s)." });
                }
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> StagingEpisode(Guid id)
        {

            var episode = await _db.Episodes
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
            var podcastEpisodes = await _db.Episodes
                .Where(p => p.PodcastId == episode.PodcastId && p.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            int episodeNum = 1;
            foreach (var epis in podcastEpisodes)
            {
                epis.EpisodeNumber = episodeNum;
                episodeNum++;
            }

            EpisodeDetailViewModel episodeDetail = new EpisodeDetailViewModel
            {
                Episode = episode,
                EpisodeNumber = episode.EpisodeNumber
            };

            return View(episodeDetail);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePublishEpisode(Guid id)
        {
            var episode = await _db.Episodes
         .Include(e => e.Podcast)  
         .FirstOrDefaultAsync(e => e.EpisodeId == id);

            if (episode == null)
            {
                return Json(new { success = false, message = "Episode not found." });
            }
            if(episode.IsActive)
            {
                episode.IsActive = false;
                episode.IsDeleted = true;

                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Episode unpublished successfully!" });

            }
            else
            {
                if (episode.Podcast!.IsDeleted)
                {
                    return Json(new { success = false, message = "Cannot publish an episode for a podcast that is inactive." });
                }

                episode.IsActive = true;
                episode.IsDeleted = false;

                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Episode published successfully!" });
            }
            
        }
    }
}
