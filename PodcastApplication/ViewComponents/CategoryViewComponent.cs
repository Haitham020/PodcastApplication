using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private AppDbContext db;
        public CategoryViewComponent(AppDbContext _db)
        {
            db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await db.Categories.Where(c => c.IsActive)
                .Select(c => new
                {
                    Category = c,
                    EpisodeCount = c.Podcasts!.Where(p => p.IsActive)
                .SelectMany(p => p.Episodes!).Count(e => e.IsActive)
                }).Take(4).ToListAsync();

            return View(categories);

        }
    }
}
