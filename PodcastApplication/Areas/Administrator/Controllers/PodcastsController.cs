using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Administrator/Podcasts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PodcastId,PodcastTitle,PodcastDescription,PodcastCoverImg,CategoryId,CreatorId,IsActive,IsDeleted,CreatedAt")] Podcast podcast)
        {
            if (ModelState.IsValid)
            {
                podcast.PodcastId = Guid.NewGuid();
                _context.Add(podcast);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", podcast.CategoryId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", podcast.CreatorId);
            return View(podcast);
        }

        // GET: Administrator/Podcasts/Edit/5
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", podcast.CategoryId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", podcast.CreatorId);
            return View(podcast);
        }

        // POST: Administrator/Podcasts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PodcastId,PodcastTitle,PodcastDescription,PodcastCoverImg,CategoryId,CreatorId,IsActive,IsDeleted,CreatedAt")] Podcast podcast)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", podcast.CategoryId);
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", podcast.CreatorId);
            return View(podcast);
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
                _context.Podcasts.Remove(podcast);
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
