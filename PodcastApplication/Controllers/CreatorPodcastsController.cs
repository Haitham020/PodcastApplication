using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NAudio.MediaFoundation;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.Controllers
{
    [Authorize(Roles = "Creator")]
    public class CreatorPodcastsController : Controller
    {
        private readonly AppDbContext _context;

        public CreatorPodcastsController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var appDbContext = _context.Podcasts
                .Include(p => p.Category)
                .Include(p => p.Creator)
                .Where(x => x.CreatorId == userId && x.IsActive)
                .OrderBy(x => x.CreatedAt);

            return View(await appDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcast = await _context.Podcasts
                .Include(p => p.Category)
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.PodcastId == id);
            if (podcast == null)
            {
                return NotFound();
            }

            return View(podcast);
        }


        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Podcast podcast, IFormFile imgFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {

                if (imgFile != null && imgFile.Length > 0)
                {
                    var extension = Path.GetExtension(imgFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                        ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                        return View(podcast);
                    }

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/podcast");

                    // Check if the directory exists, if not, create it
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(folderPath, imgFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }

                    // Assign the file name to the episode's image property
                    podcast.PodcastCoverImg = imgFile.FileName;

                }




                podcast.PodcastId = Guid.NewGuid();
                podcast.CreatedAt = DateTime.Now;
                podcast.IsActive = true;
                podcast.IsDeleted = false;
                podcast.IsPublic = false;
                podcast.CreatorId = userId;
                _context.Add(podcast);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View(podcast);
        }

        // GET: CreatorPodcasts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Creator = new SelectList(_context.Users, "Id", "UserName");

            return View(podcast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Podcast podcast, IFormFile imgFile)
        {
            if (id != podcast.PodcastId)
            {
                return NotFound();
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

            if (imgFile != null && imgFile.Length > 0)
            {
                var extension = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("imgFile", "Only image files (jpg, jpeg, png, gif) are allowed.");
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/podcast", imgFile.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imgFile.CopyToAsync(stream);
                }
                podcast.PodcastCoverImg = imgFile.FileName;
            }
            else
            {
                podcast.PodcastCoverImg = _context.Podcasts.AsNoTracking()
                                   .Where(p => p.PodcastId == id)
                                   .Select(p => p.PodcastCoverImg)
                                   .FirstOrDefault();
            }
           

            try
            {
                podcast.PodcastTitle = podcast.PodcastTitle;
                podcast.PodcastDescription = podcast.PodcastDescription;
                podcast.IsPublic = podcast.IsPublic;
                podcast.IsDeleted = podcast.IsDeleted;
                podcast.CreatedAt = DateTime.Now;

                _context.Update(podcast);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PodcastExists(podcast.PodcastId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: CreatorPodcasts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podcast = await _context.Podcasts
                .Include(p => p.Category)
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.PodcastId == id);
            if (podcast == null)
            {
                return NotFound();
            }

            return View(podcast);
        }

        // POST: CreatorPodcasts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var podcast = await _context.Podcasts
                .Include(p => p.Episodes)
                .FirstOrDefaultAsync(p => p.PodcastId == id);

            if (podcast != null)
            {
                podcast.IsActive = false;
                podcast.IsDeleted = true;

                foreach (var episode in podcast.Episodes!)
                {
                    episode.IsDeleted = true;
                    episode.IsActive = false;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PodcastExists(Guid id)
        {
            return _context.Podcasts.Any(e => e.PodcastId == id);
        }
    }
}
