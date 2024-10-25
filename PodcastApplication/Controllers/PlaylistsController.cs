using System;
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
                .Where(p => p.UserId == userId && p.IsActive)
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
                playlist.IsPublic = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyPlaylists));
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

        [HttpGet("/Playlists/SavedItemsInPlaylist/{playlistId}")]
        public async Task<IActionResult> SavedItemsInPlaylist(int playlistId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                 .Include(p => p.PlaylistItems!.Where(x => x.IsActive))
                 .ThenInclude(pi => pi.Podcast)
                 .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist == null)
            {
                return NotFound("playlist is null!");
            }

            return View(playlist);
        }

        [HttpGet]
        public async Task<IActionResult> DeletePlaylistItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlistItem = await _context.PlaylistItems
                .Include(p => p.Podcast)
                .FirstOrDefaultAsync(m => m.PlaylistItemId == id);

            if (playlistItem == null)
            {
                return NotFound();
            }

            return View(playlistItem);


        }


        [HttpPost, ActionName("DeletePlaylistItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteItem(int id)
        {
            var playlistItem = await _context.PlaylistItems.FindAsync(id);
            if (playlistItem != null)
            {
                _context.Remove(playlistItem);
                await _context.SaveChangesAsync();

            }


            int playlistId = playlistItem!.PlaylistId;

            return RedirectToAction("SavedItemsInPlaylist", "Playlists", new { playlistId });
        }

        public async Task<IActionResult> PublicPlaylists()
        {
            var publicPlaylists = await _context.Playlists
                .Include(x => x.User)
                .Where(x => x.IsPublic)
                .ToListAsync();

            if(publicPlaylists == null)
            {
                return Content("PLaylist no longer exists");
            }
            return View(publicPlaylists);       
        }

        public async Task<IActionResult> SavedPublicPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            var savedPlaylists = await _context.SavedPlaylists
                .Include(x => x.User)
                .Include(p => p.Playlist)
                .ThenInclude(u => u!.User)
                .Where(x => x.UserId == userId && x.Playlist!.IsPublic)
                .Select(x => x.Playlist)
                .ToListAsync();

            return View(savedPlaylists);
        }

    }
}
