using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Diagnostics;

namespace PodcastApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;


        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            if(users == null)
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
        public async Task<IActionResult> About()
        {
            var creators = await (from user in _db.Users
                                  join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                  join role in _db.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "Creator"
                                  select user)
                           .Take(4)
                           .ToListAsync();
            return View(creators);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
