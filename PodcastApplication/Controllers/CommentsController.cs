using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Data;
using PodcastApplication.Models;
using System.Security.Claims;

public class CommentsController : Controller
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult AllComments(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
        {
            return NoContent();
        }

        var comments = _context.Comments
            .Include(x => x.User)
            .Where(c => c.EpisodeId == id)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        ViewBag.EpisodeId = id;

        return View(comments);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(Guid id, string commentText)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("You must be logged in to post comments.");
        }

        if (string.IsNullOrWhiteSpace(commentText))
        {
            return BadRequest("Comment text cannot be empty.");
        }

        var newComment = new Comment
        {
            UserId = userId,
            EpisodeId = id,
            CommentText = commentText,
            CreatedAt = DateTime.Now
        };

        _context.Comments.Add(newComment);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new
            {
                id = newComment.CommentId,
                userName = User.Identity!.Name,
                commentText,
                createdAt = newComment.CreatedAt.ToString("g") 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
        
    }


    [HttpPost]
    public IActionResult EditComment(int commentId, string updatedText)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var comment = _context.Comments.Find(commentId);
        if (comment == null) return NotFound();

        if (comment.UserId != userId)
        {
            return Forbid();
        }

        comment.CommentText = updatedText;
        _context.SaveChanges();

        return Ok();
    }

    // Delete Comment
    [HttpPost]
    public IActionResult DeleteComment(int commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var comment = _context.Comments.Find(commentId);
        if (comment == null) return NotFound();

        if (comment.UserId != userId)
        {
            return Forbid();
        }

        _context.Comments.Remove(comment);
        _context.SaveChanges();

        return Ok(); 
    }
}
