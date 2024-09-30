using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PodcastApplication.Data;
using PodcastApplication.Models.StaticData;
using PodcastApplication.Models;
using PodcastApplication.Areas.Administrator.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace PodcastApplication.Areas.Administrator.Controllers
{
    [Area("Administrator")]
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
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                    Country = model.Country
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

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
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "ControlPanel");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error Logging In, Please Try Again.");
                    }
                    return View(model);
                }

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
