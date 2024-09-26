using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int RatingValue { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Podcast")]
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

    }
}
