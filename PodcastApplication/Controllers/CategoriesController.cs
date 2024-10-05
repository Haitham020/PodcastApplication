using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private AppDbContext db;
        public CategoriesController(AppDbContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> AllCategories()
        {
            var categories = await db.Categories.Where(c => c.IsActive)
                .Select(c => new
            {
                Category = c,
                EpisodeCount = c.Podcasts!.Where(p => p.IsActive)
                .SelectMany(p => p.Episodes!).Count(e => e.IsActive)
            }).ToListAsync();

            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> CategoryPodcasts(int? id)
        {
            var category = await db.Categories
            .Include(c => c.Podcasts!.Where(p => p.IsActive))!
            .ThenInclude(x => x.Creator)
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


    }
}
