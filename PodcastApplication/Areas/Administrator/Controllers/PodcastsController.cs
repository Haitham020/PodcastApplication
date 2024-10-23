using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Admin")]
    public class PodcastsController : Controller
    {
        private readonly AppDbContext _context;

        public PodcastsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Administrator/Podcasts
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Podcasts.Include(p => p.Category).Include(p => p.Creator);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Administrator/Podcasts/Details/5
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

        // GET: Administrator/Podcasts/Create
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Creator = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Administrator/Podcasts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Podcast podcast, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                var extension = Path.GetExtension(imgFile.FileName).ToLower();
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg"};


                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                    return View(podcast);
                }


                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/images/podcast", imgFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    podcast.PodcastCoverImg = imgFile.FileName;
                }
                podcast.PodcastId = Guid.NewGuid();
                _context.Add(podcast);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Creator = new SelectList(_context.Users, "Id", "UserName");
            return View(podcast);
        }

       
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
            ViewBag.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
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

            // Check if image file was provided
            if (imgFile != null && imgFile.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile", imgFile.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imgFile.CopyToAsync(stream);
                }
                podcast.PodcastCoverImg = imgFile.FileName; // Assign the file name only if a new file is uploaded
            }

            try
            {
                podcast.Creator = podcast.Creator;

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


        // GET: Administrator/Podcasts/Delete/5
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

        // POST: Administrator/Podcasts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast != null)
            {
                podcast.IsDeleted = true;
                podcast.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PodcastExists(Guid id)
        {
            return _context.Podcasts.Any(e => e.PodcastId == id);
        }
    }
}
