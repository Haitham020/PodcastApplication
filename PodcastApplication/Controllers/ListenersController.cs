using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

namespace PodcastApplication.Controllers
{
    [Authorize(Roles = "Listener")]
    public class ListenersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _db;
        public ListenersController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> ListenerProfile()
        {
           
            var listenerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (listenerId == null)
            {
                return NotFound();
            }

            var listener = await _userManager.FindByIdAsync(listenerId);

            if (listener == null)
            {
                return NotFound();
            }

            
            return View(listener);
        }

    }
}
