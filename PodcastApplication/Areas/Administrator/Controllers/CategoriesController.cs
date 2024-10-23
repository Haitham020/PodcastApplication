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
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Administrator/Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Administrator/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Administrator/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                var extension = Path.GetExtension(imgFile.FileName).ToLower();
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };


                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                    return View(category);
                }
                if (imgFile != null && imgFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/images/topics", imgFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }
                    category.CategoryImg = imgFile.FileName;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Administrator/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Administrator/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imgFile,Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var extension = Path.GetExtension(imgFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                        return View(category);
                    }
                    if (imgFile != null && imgFile.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/images/topics", imgFile.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await imgFile.CopyToAsync(stream);
                        }
                        category.CategoryImg = imgFile.FileName;

                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Administrator/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Administrator/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                category.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
