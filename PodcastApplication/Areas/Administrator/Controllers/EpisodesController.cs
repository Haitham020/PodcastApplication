﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using NAudio.Wave;

namespace PodcastApplication.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Admin")]
    public class EpisodesController : Controller
    {
        private readonly AppDbContext _context;

        public EpisodesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Administrator/Episodes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Episodes.Include(e => e.Podcast);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Administrator/Episodes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(m => m.EpisodeId == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // GET: Administrator/Episodes/Create
        public IActionResult Create()
        {
            ViewBag.Podcast = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Episode episode, IFormFile audioFile, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                if (audioFile != null && audioFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/audio", audioFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await audioFile.CopyToAsync(stream);
                    }

                    episode.AudioFile = audioFile.FileName;

                    using (var audioFileReader = new AudioFileReader(filePath))
                    {
                        episode.EpisodeDuration = TimeSpan.FromSeconds(Math.Round(audioFileReader.TotalTime.TotalSeconds));
                    }

                }
                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/images/episode", imgFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    episode.EpisodeCoverImg = imgFile.FileName;
                }

                episode.EpisodeNumber = 1;
                episode.IsActive = true;
                episode.IsDeleted = false;

                episode.EpisodeId = Guid.NewGuid();
                _context.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Podcast = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
            return View(episode);
        }

        // GET: Administrator/Episodes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastId", episode.PodcastId);
            return View(episode);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Episode episode, IFormFile imgFile)
        {
            if (id != episode.EpisodeId)
            {
                return NotFound();
            }

            // Check if image file was provided
            if (imgFile != null && imgFile.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/episode", imgFile.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imgFile.CopyToAsync(stream);
                }
                episode.EpisodeCoverImg = imgFile.FileName; // Assign the file name only if a new file is uploaded
            }

            try
            {
                // Update episode details without requiring a new image file every time
                _context.Update(episode);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(episode.EpisodeId))
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


        // GET: Administrator/Episodes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(m => m.EpisodeId == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Administrator/Episodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                episode.IsDeleted = true;
                episode.IsActive = false;
                episode.IsPublic = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeExists(Guid id)
        {
            return _context.Episodes.Any(e => e.EpisodeId == id);
        }
    }
}
