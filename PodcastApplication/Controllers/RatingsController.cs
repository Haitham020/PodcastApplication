using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class RatingsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public RatingsController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        [HttpPost]
        public async Task<IActionResult> SubmitRating(Guid podcastId, int ratingValue)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var existingRating = await _db.Ratings
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PodcastId == podcastId);

            if (existingRating != null)
            {
                existingRating.RatingValue = ratingValue;
               
            }
            else
            {
                Rating rating = new Rating
                {
                    UserId = userId,
                    PodcastId = podcastId,
                    RatingValue = ratingValue,
                };
                await _db.Ratings.AddAsync(rating);
            }
            await _db.SaveChangesAsync();

            var averageRating = await _db.Ratings
                .Where(p => p.PodcastId == podcastId)
                .AverageAsync(x => x.RatingValue);
            averageRating = Math.Round(averageRating, 1);

            return Json(new { success = true, averageRating, existingRatingValue = existingRating?.RatingValue ?? ratingValue });
        }
    }
}
