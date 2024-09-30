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
                .OrderByDescending(e => e.EpisodeLikes!.Count())
                .Take(3).Include(p => p.Podcast)
                .ThenInclude(x => x!.Creator)
                .ToListAsync();

            return View(trendingEpisodes);
        }
    }
}
