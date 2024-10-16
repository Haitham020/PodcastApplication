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
    public IActionResult GetComments(Guid episodeId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
        {
            return NoContent();
        }

        var comments = _context.Comments
            .Include(x => x.User)
            .Where(c => c.EpisodeId == episodeId)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        return PartialView("_CommentsList", comments);
    }

    // Add comment (already implemented)
    [HttpPost]
    public IActionResult AddComment(Guid episodeId, string commentText)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NoContent();
        }

        var newComment = new Comment
        {
            UserId = userId,
            EpisodeId = episodeId,
            CommentText = commentText,
            CreatedAt = DateTime.Now
        };

        _context.Comments.Add(newComment);
        _context.SaveChanges();

        return PartialView("_SingleComment", newComment);
    }

    // Edit Comment
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

        return PartialView("_SingleComment", comment); // Return the updated comment
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
