using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Subscription : SharedProperties
    {
        public int SubscriptionId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Podcast")]
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

    }
}
