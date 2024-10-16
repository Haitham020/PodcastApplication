using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using PodcastApplication.Data;
using PodcastApplication.Models;
using AssemblyAI;
using AssemblyAI.Transcripts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace PodcastApplication.Controllers
{
    [Authorize(Roles = "Creator")]
    public class CreatorEpisodesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreatorEpisodesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CreatorEpisodes
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var appDbContext = _context.Episodes
                .Include(e => e.Podcast)
                .Where(x => x.Podcast!.CreatorId == userId);

            return View(await appDbContext.ToListAsync());
        }

        // GET: CreatorEpisodes/Details/5
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

        // GET: CreatorEpisodes/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var creatorPodcasts = _context.Podcasts
                .Where(x => x.CreatorId == userId);

            ViewData["PodcastId"] = new SelectList(creatorPodcasts, "PodcastId", "PodcastTitle");
            return View();
        }

        // POST: CreatorEpisodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Episode episode, IFormFile audioFile)
        {
            if (ModelState.IsValid)
            {
                if (audioFile != null && audioFile.Length > 0)
                {
                    var extension = Path.GetExtension(audioFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg", ".aac" };


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                        ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
                        return View(episode);
                    }

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


                episode.EpisodeNumber = 1;
                episode.IsActive = false;
                episode.IsDeleted = true;
                episode.CreatedAt = DateTime.Now;

                episode.EpisodeId = Guid.NewGuid();
                _context.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
            return View(episode);
        }



        [HttpPost]
        public async Task<IActionResult> GetEpisodeDuration(IFormFile audioFile)
        {
            if (audioFile != null && audioFile.Length > 0)
            {

                var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(audioFile.FileName));
                try
                {

                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await audioFile.CopyToAsync(stream);
                    }

                    using (var audioFileReader = new AudioFileReader(tempFilePath))
                    {
                        var duration = TimeSpan.FromSeconds(Math.Round(audioFileReader.TotalTime.TotalSeconds));
                        return Json(new { episodeDuration = duration.ToString(@"hh\:mm\:ss") });
                    }
                }
                catch (Exception)
                {
                    return Json(new { error = $"An error occurred uploading the audio file. Try again" });
                }
                finally
                {
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(tempFilePath);
                        }
                        catch (IOException ioEx)
                        {
                            ModelState.AddModelError("", $"Error deleting temporary file: {ioEx.Message}");
                        }
                    }
                }
            }

            return Json(new { error = "Invalid audio file." });
        }




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
            ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
            return View(episode);
        }

        // POST: CreatorEpisodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Episode episode, IFormFile audioFile)
        {
            if (id != episode.EpisodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg", ".aac" };
                    var extension = Path.GetExtension(audioFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                        ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
                        return View(episode);
                    }

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
            ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
            return View(episode);
        }

        // GET: CreatorEpisodes/Delete/5
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

        // POST: CreatorEpisodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                episode.IsActive = false;
                episode.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeExists(Guid id)
        {
            return _context.Episodes.Any(e => e.EpisodeId == id);
        }

        [HttpPost]
        public async Task<IActionResult> Transcribe(Guid episodeId)
        {
            var episode = await _context.Episodes.FindAsync(episodeId);
            if (episode == null || string.IsNullOrEmpty(episode.AudioFile))
            {
                return NotFound("Episode or audio file not found");
            }

            var apiKey = "7d2d8eb3f43f402fa53b42566eedb3e3";

            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "audio", episode.AudioFile);

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound("Audio file not found on the server");
            }

            var client = new AssemblyAIClient(apiKey);

            var uploadedFile = await client.Files.UploadAsync(new FileInfo(physicalPath));
            var fileUrl = uploadedFile.UploadUrl;

            var transcriptParams = new TranscriptParams
            {
                AudioUrl = fileUrl,
                SpeechModel = SpeechModel.Nano,
                LanguageDetection = true
            };

            var transcript = await client.Transcripts.TranscribeAsync(transcriptParams);

            while (transcript.Status != TranscriptStatus.Completed)
            {
                await Task.Delay(5000);

                transcript = await client.Transcripts.GetAsync(transcript.Id);

                if (transcript.Status == TranscriptStatus.Error)
                {
                    return StatusCode(500, "Transcription failed.");
                }
            }

            episode.Transcript = transcript.Text; 
            _context.Episodes.Update(episode);
            await _context.SaveChangesAsync();

            return Json(new { text = transcript.Text });
        }
        [HttpPost]
        public async Task<IActionResult> SaveTranscript(Guid episodeId, string transcript)
        {
            var episode = await _context.Episodes.FindAsync(episodeId);
            if (episode == null)
            {
                return NotFound("Episode not found");
            }

            episode.Transcript = transcript;
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
