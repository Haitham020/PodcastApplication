namespace PodcastApplication.Models
{
    public class Podcast : SharedProperties
    {
        public Guid PodcastId { get; set; }
        public string? PodcastTitle { get; set; }
        public string? PodcastDescription { get; set; }
        public string? PodcastCoverImg { get; set; }

        public ICollection<PodcastLike>? PodcastLikes { get; set; }
        
    }
}
