using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using PodcastApplication.Models.ViewModels;
using System.Linq;

namespace PodcastApplication.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;


        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return NoContent();
            }

            var podcasts = await _context.Podcasts
                .Include(x => x.Creator)
                .Where(p => p.IsPublic &&  p.PodcastTitle!.Contains(query))
                .AsNoTracking()
                .ToListAsync();

            var episodes = await _context.Episodes
                .Include(x => x.Podcast)
                .Where(e => e.IsPublic &&  e.EpisodeTitle!.Contains(query))
                .AsNoTracking()
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive && c.CategoryName!.Contains(query))
                .AsNoTracking()
                .ToListAsync();

            var playlists = await _context.Playlists
                .Where(c => c.IsPublic && c.PlaylistName!.Contains(query))
                .AsNoTracking()
                .ToListAsync();

           
            SearchViewModel searchResults = new SearchViewModel
            {
                Podcasts = podcasts,
                Episodes = episodes,
                Categories = categories,
                Playlists = playlists
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
                .Where(p => p.PodcastTitle!.StartsWith(query) && p.IsPublic)
                .Select(p => p.PodcastTitle) 
                .ToListAsync();

            var episodeSuggestions = await _context.Episodes
                .Where(e => e.EpisodeTitle!.StartsWith(query) && e.IsPublic)
                .Select(e => e.EpisodeTitle)
                .ToListAsync();

            var categorySuggestions = await _context.Categories
                .Where(c => c.CategoryName!.StartsWith(query) && c.IsActive)
                .Select(c => c.CategoryName)
                .ToListAsync();

            var playlistSuggestions = await _context.Playlists
                .Include(x => x.User)
                .Where(c => c.PlaylistName!.StartsWith(query))
                .Select(c => c.PlaylistName)
                .ToListAsync();


            var allSuggestions = podcastSuggestions.Concat(episodeSuggestions).Concat(categorySuggestions).Concat(playlistSuggestions);

            return PartialView("_Suggestions", allSuggestions);
        }

    }

}
