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
                .Where(x => x.CreatorId == userId);

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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: CreatorPodcasts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Podcast podcast)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                podcast.PodcastId = Guid.NewGuid();
                podcast.CreatedAt = DateTime.Now;
                podcast.IsActive = false;
                podcast.IsDeleted = true;
                podcast.CreatorId = userId;
                _context.Add(podcast);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
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
            return View(podcast);
        }

        // POST: CreatorPodcasts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Podcast podcast)
        {
            if (id != podcast.PodcastId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View(podcast);
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
