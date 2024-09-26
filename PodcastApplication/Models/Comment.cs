namespace PodcastApplication.Models
{
    public class Comment : SharedProperties
    {
        public int CommentId { get; set; }
        public string? CommentText { get; set; }
    }
}
