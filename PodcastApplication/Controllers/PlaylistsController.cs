﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;

namespace PodcastApplication.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaylistsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> MyPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(playlists);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Playlist playlist)
        {
            if (ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(User);

                playlist.UserId = userId;
                playlist.CreatedAt = DateTime.Now;
                playlist.IsActive = true;
                playlist.IsDeleted = false;

                if (userId == null)
                {
                    return NotFound();
                }

                _context.Add(playlist);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyPlaylists", "Playlists");
            }

            return View();
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Playlist playlist)
        {
            if (id != playlist.PlaylistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.PlaylistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyPlaylists));
            }

            return View(playlist);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.PlaylistId == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                playlist.IsActive = false;
                playlist.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.PlaylistId == id);
        }

        //Adding Items to Playlists:
        [HttpPost]
        public IActionResult AddToPlaylists(Guid podcastId, List<int> playlistIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest(new { success = false, message = "User not authenticated." });
            }

            if (playlistIds == null || !playlistIds.Any())
            {
                return Json(new { success = false, message = "No playlists selected." });
            }

            var duplicateMessages = new List<string>();

            foreach (var playlistId in playlistIds)
            {
                var exists = _context.PlaylistItems
            .Any(pi => pi.PlaylistId == playlistId && pi.PodcastId == podcastId);

                if (exists)
                {
       
                    var playlistName = _context.Playlists
                        .Where(p => p.PlaylistId == playlistId)
                        .Select(p => p.PlaylistName)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(playlistName))
                    {
                        
                        return Json(new { success = false });
                    }
                    continue;
                }
                PlaylistItem playlistItem = new PlaylistItem
                {
                    PlaylistId = playlistId,
                    PodcastId = podcastId,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };

                _context.PlaylistItems.Add(playlistItem);
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> SavedItemsInPlaylist(int playlistId)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            var playlist = await _context.Playlists
                 .Include(p => p.PlaylistItems!)
                 .ThenInclude(pi => pi.Podcast)
                 .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UserId == userId);

            if (playlist == null)
            {
                Debug.WriteLine($"Playlist with ID {playlistId} not found for user {userId}.");
                return NotFound("playlist is null!");
            }

            return View(playlist);
        }

    }
}
