using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.ViewComponents
{
    public class CreatorViewComponent : ViewComponent
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        public CreatorViewComponent(UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public IViewComponentResult Invoke()
        {
            return View(userManager.GetUsersInRoleAsync("Creator").Result.ToList());
        }
    }
}
