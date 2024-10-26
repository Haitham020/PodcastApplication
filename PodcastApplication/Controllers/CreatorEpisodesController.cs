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
using System.Diagnostics;

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
                .OrderBy(e => e.CreatedAt)
                .Where(x => x.Podcast!.CreatorId == userId && x.IsActive);

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
                .Where(x => x.CreatorId == userId && x.IsActive);

            ViewData["PodcastId"] = new SelectList(creatorPodcasts, "PodcastId", "PodcastTitle");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Episode episode, IFormFile audioFile, IFormFile imgFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

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



                if (imgFile != null && imgFile.Length > 0)
                {
                    var extension = Path.GetExtension(imgFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AudioFile", "Invalid file type. Please upload only audio files (mp3, wav, etc.).");
                        ViewData["PodcastId"] = new SelectList(_context.Podcasts, "PodcastId", "PodcastTitle");
                        return View(episode);
                    }

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/episodes");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(folderPath, imgFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imgFile.CopyToAsync(stream);
                    }

                    episode.EpisodeCoverImg = imgFile.FileName;

                }



                episode.EpisodeNumber = 1;
                episode.IsActive = true;
                episode.IsDeleted = false;
                episode.IsPublic = false;
                episode.CreatedAt = DateTime.Now;

                episode.EpisodeId = Guid.NewGuid();
                _context.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var creatorPodcasts = _context.Podcasts
                            .Where(x => x.CreatorId == userId && x.IsActive);

            ViewData["PodcastId"] = new SelectList(creatorPodcasts, "PodcastId", "PodcastTitle"); 
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }

            var creatorPodcasts = _context.Podcasts
                           .Where(x => x.CreatorId == userId && x.IsActive);

            ViewData["PodcastId"] = new SelectList(creatorPodcasts, "PodcastId", "PodcastTitle");
            
            return View(episode);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Episode episode, IFormFile audioFile, IFormFile imgFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }


            if (id != episode.EpisodeId)
            {
                return NotFound();
            }


            var allowedAudioExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg", ".aac" };

            if (audioFile != null && audioFile.Length > 0)
            {
                var audioExtension = Path.GetExtension(audioFile.FileName).ToLowerInvariant();

                if (!allowedAudioExtensions.Contains(audioExtension))
                {
                    throw new Exception("Invalid audio file type. Please upload only mp3, wav, flac, ogg, or aac files.");
                }

                var audioFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio", audioFile.FileName);
                using (var stream = System.IO.File.Create(audioFilePath))
                {
                    await audioFile.CopyToAsync(stream);
                }

                episode.AudioFile = audioFile.FileName;

                using (var audioFileReader = new AudioFileReader(audioFilePath))
                {
                    episode.EpisodeDuration = TimeSpan.FromSeconds(Math.Round(audioFileReader.TotalTime.TotalSeconds));
                }
            }
            else
            {
                episode.AudioFile = _context.Episodes.AsNoTracking()
                                                       .Where(p => p.EpisodeId == id)
                                                       .Select(p => p.AudioFile)
                                                       .FirstOrDefault();
            }

            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (imgFile != null && imgFile.Length > 0)
            {
                var imageExtension = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                if (!allowedImageExtensions.Contains(imageExtension))
                {
                    throw new Exception("Invalid image file type. Please upload only jpg, jpeg, png, or gif files.");
                }

                var imgFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/episodes", imgFile.FileName);
                using (var stream = System.IO.File.Create(imgFilePath))
                {
                    await imgFile.CopyToAsync(stream);
                }

                episode.EpisodeCoverImg = imgFile.FileName;
            }
            else
            {

                episode.EpisodeCoverImg = _context.Episodes.AsNoTracking()
                                   .Where(p => p.EpisodeId == id)
                                   .Select(p => p.EpisodeCoverImg)
                                   .FirstOrDefault();

            }
            try
            {
               
                episode.CreatedAt = DateTime.Now;

                _context.Update(episode);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                var creatorPodcasts = _context.Podcasts
                           .Where(x => x.CreatorId == userId && x.IsActive);

                ViewData["PodcastId"] = new SelectList(creatorPodcasts, "PodcastId", "PodcastTitle");
                return View(episode);
            }
            return RedirectToAction(nameof(Index));
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
                episode.IsPublic = false;
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
        public IActionResult Recorder()
        {
            return View();
        }
        public async Task<IActionResult> DeletedEpisodes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var appDbContext = _context.Episodes
                .Include(p => p.Podcast)
                .Where(x => x.Podcast!.CreatorId == userId && x.IsDeleted)
                .OrderBy(x => x.CreatedAt);

            return View(await appDbContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> RestoreEpisode(Guid id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                episode.IsActive = true;
                episode.IsDeleted = false;
                episode.IsPublic = false;
            }
            else
            {
                return View(episode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
