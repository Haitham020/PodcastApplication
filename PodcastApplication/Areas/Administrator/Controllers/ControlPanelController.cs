using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ControlPanelController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _db;
        public ControlPanelController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;

        }
        
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                var users = await _userManager.Users.ToListAsync();
                if (users == null)
                {
                    return Content("No Users");
                }
                ViewBag.CreatorUsers = users.Count(user =>
                _userManager.IsInRoleAsync(user, "Creator").Result);

                ViewBag.ListenerUsers = users.Count(user =>
                _userManager.IsInRoleAsync(user, "Listener").Result);

                var Podcasts = _db.Podcasts.Count();
                var Episodes = _db.Episodes.Count();

                ViewBag.Podcasts = Podcasts;
                ViewBag.Episodes = Episodes;

                return View();
            }
            return RedirectToAction("Login", "Account");

        }
    }
}
