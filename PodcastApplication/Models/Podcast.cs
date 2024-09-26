using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Podcast : SharedProperties
    {
        public Guid PodcastId { get; set; }
        public string? PodcastTitle { get; set; }
        public string? PodcastDescription { get; set; }
        public string? PodcastCoverImg { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Episode>? Episodes { get; set; }
        public ICollection<Subscription>? Subscriptions { get; set; }
        public ICollection<Rating>? Ratings { get; set; }

    }
}
