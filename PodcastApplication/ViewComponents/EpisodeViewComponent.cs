using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;

namespace PodcastApplication.ViewComponents
{
    public class EpisodeViewComponent : ViewComponent
    {
        private AppDbContext db;
        public EpisodeViewComponent(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var episodes = await db.Episodes
                .Include(f => f.Favorites)
                .Include(c => c.Comments)
                .Include(i => i.EpisodeLikes)
                .Include(x => x.EpisodeListeners)
                .Include(x => x.Podcast)
                .ThenInclude(x => x!.Creator)
                .OrderBy(e => e.CreatedAt)
                .Where(e => e.IsActive)
                .ToListAsync();

            foreach(var podcast in episodes.GroupBy(p => p.PodcastId))
            {
   
                int episodeNum = 1;
                foreach(var episode in podcast)
                {
                    episode.EpisodeNumber = episodeNum;
                    episodeNum++;
                }
            }
            var displayedEpisodes = episodes.Take(2);
            return View(displayedEpisodes);
        } 
        
    }
}
