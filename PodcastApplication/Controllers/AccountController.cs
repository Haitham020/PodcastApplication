using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PodcastApplication.Models.ViewModels;
using PodcastApplication.Models;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models.StaticData;

namespace PodcastApplication.Controllers
{
    public class AccountController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _db;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            AppDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {

            RegisterViewModel model = new RegisterViewModel
            {
                Countries = CountryList.GetCountries()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string userRole)
        {
            if (ModelState.IsValid)
            {
                var existedEmail = await _userManager.FindByEmailAsync(model.Email!);
                if (existedEmail != null)
                {
                    ModelState.AddModelError("Email", "Password or Email credentials wrong");
                    return View(model);
                }

                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Age = model.Age,
                    Country = model.Country,
                    DateJoined = DateTime.Now

                };
                if (userRole == "Creator")
                {
                    user.Active = false;
                    user.InActive = true;
                }
                else if (userRole == "Listener")
                {
                    user.Active = true;
                    user.InActive = false;
                }

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userRole);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Reload the roles if the model state is invalid
            model.Roles = _roleManager.Roles.ToList();
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email!);
                if (user != null && user.Active == true)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        ModelState.AddModelError("", "Error Logging In, Check Credentials");
                    }
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Please Wait Until Admin Approval");
                }

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Creators()
        {
            var creators = _userManager
                .GetUsersInRoleAsync("Creator")
                .Result
                .ToList();
            var activeCreators = creators
                .Where(c => c.Active)
                .ToList();

            return View(activeCreators);
        }
        [HttpGet]
        public async Task<IActionResult> CreatorDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creator = await _userManager.Users
                                  .Include(u => u.Podcasts!.Where(u => u.IsPublic))
                                  .FirstOrDefaultAsync(u => u.Id == id);
            if(creator == null)
            {
                return NotFound();
            }

            return View(creator);

        }
    }

}
