using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    public class SubscriptionsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public SubscriptionsController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubscription(Guid podcastId, bool isSubscribing)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
            }

            if(isSubscribing)
            {
                var subscriptionExists = await _db.Subscriptions
                .AnyAsync(x => x.UserId == userId && x.PodcastId == podcastId);

                if (!subscriptionExists)
                {
                    Subscription subscription = new Subscription
                    {
                        UserId = userId,
                        PodcastId = podcastId,
                        CreatedAt = DateTime.Now
                    };

                    await _db.Subscriptions.AddAsync(subscription);
                    await _db.SaveChangesAsync();

                }
            }
            else
            {
                var subbed = await _db.Subscriptions
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.PodcastId == podcastId);
                if(subbed != null)
                {
                    _db.Subscriptions.Remove(subbed);
                    await _db.SaveChangesAsync();
                }
           
            }

          
            var subCount = await _db.Subscriptions
                .CountAsync(x => x.PodcastId == podcastId);

            return Json(new { subscriptionCount = subCount });

        }

        [HttpGet]
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var mySubs = await _db.Subscriptions
                .Where(x => x.UserId == userId)
                .Include(p => p.Podcast)
                .ThenInclude(x => x!.Creator)
                .Select(x => x.Podcast)
                .ToListAsync();

            return View(mySubs);
        }
    }
}
