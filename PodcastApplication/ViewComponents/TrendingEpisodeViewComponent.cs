using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;

namespace PodcastApplication.ViewComponents
{
    public class TrendingEpisodeViewComponent : ViewComponent
    {
        private AppDbContext db;
        public TrendingEpisodeViewComponent(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var trendingEpisodes = await db.Episodes
                .Include(x => x.EpisodeLikes)
                .OrderByDescending(e => e.EpisodeLikes!.Count())
                .Include(c => c.Comments)
                .Include(x => x.EpisodeListeners)
                .Include(p => p.Podcast)
                .ThenInclude(x => x!.Creator)
                .Where(x => x.IsPublic)
                .Take(3)
                .ToListAsync();


            return View(trendingEpisodes);
        }
    }
}
