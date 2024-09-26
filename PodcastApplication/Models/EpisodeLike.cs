using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class EpisodeLike : SharedProperties
    {
        public int EpisodeLikeId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Episode")]
        public Guid EpisodeId { get; set; }
        public Episode? Episode { get; set; }
    }
}
