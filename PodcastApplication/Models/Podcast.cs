using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Podcast : SharedProperties
    {
        [Key]
        public Guid PodcastId { get; set; }

        [Required(ErrorMessage = "Podcast title is required")]
        [MaxLength(200, ErrorMessage = "Podcast title cannot exceed 200 characters")]
        public string? PodcastTitle { get; set; }

        [Required(ErrorMessage = "Podcast description is required")]
        [MaxLength(1000, ErrorMessage = "Podcast description cannot exceed 1000 characters")]
        public string? PodcastDescription { get; set; }
        public string? PodcastCoverImg { get; set; }
        public bool IsPublic {  get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? CreatorId { get; set; }
        public ApplicationUser? Creator { get; set; }

        public ICollection<Episode>? Episodes { get; set; }
        public ICollection<Subscription>? Subscriptions { get; set; }
        public ICollection<Rating>? Ratings { get; set; }

    }
}
