using System.ComponentModel.DataAnnotations;

namespace PodcastApplication.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int RatingValue { get; set; }

    }
}
