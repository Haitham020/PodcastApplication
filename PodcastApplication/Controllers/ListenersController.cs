using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public async Task<IActionResult> EditProfile(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(string id, IFormFile imgFile, ApplicationUser user)
        {
            var existingUser = await _db.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.UserName = user.UserName;
            existingUser.ProfileBio = user.ProfileBio;


            try
            {
                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/images/profile", imgFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    user.ProfileImg = imgFile.FileName;
                }

                _db.Update(existingUser);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (id != user.Id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("ListenerProfile", "Listeners");
        }
            



    }
}
