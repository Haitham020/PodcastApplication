using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models.ViewModels;

namespace PodcastApplication.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); 
            }

            var podcasts = await _context.Podcasts
                .Where(p => p.PodcastTitle!.Contains(query))
                .ToListAsync();

            var episodes = await _context.Episodes
                .Where(e => e.EpisodeTitle!.Contains(query))
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.CategoryName!.Contains(query))
                .ToListAsync();

            SearchViewModel searchResults = new SearchViewModel
            {
                Podcasts = podcasts,
                Episodes = episodes,
                Categories = categories
            };

            return View("SearchResults", searchResults); 
        }


        public async Task<IActionResult> GetSuggestions(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Content(""); 
            }

            var podcastSuggestions = await _context.Podcasts
                .Where(p => p.PodcastTitle!.StartsWith(query))
                .Select(p => p.PodcastTitle) 
                .ToListAsync();

            var episodeSuggestions = await _context.Episodes
                .Where(e => e.EpisodeTitle!.StartsWith(query))
                .Select(e => e.EpisodeTitle)
                .ToListAsync();

            var categorySuggestions = await _context.Categories
                .Where(c => c.CategoryName!.StartsWith(query))
                .Select(c => c.CategoryName)
                .ToListAsync();

            var allSuggestions = podcastSuggestions.Concat(episodeSuggestions).Concat(categorySuggestions);

            return PartialView("_Suggestions", allSuggestions);
        }

    }

}
