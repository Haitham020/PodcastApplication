namespace PodcastApplication.Models
{
    public class Episode : SharedProperties
    {
        public Guid EpisodeId { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? EpisodeDescription { get; set; }
        public string? AudioFile { get; set; }
        public TimeSpan EpisodeDuration { get; set; }

        public ICollection<EpisodeLike>? EpisodeLikes { get; set; }

    }
}
