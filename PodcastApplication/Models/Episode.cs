using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Episode : SharedProperties
    {
        [Key]
        public Guid EpisodeId { get; set; }

        [Required(ErrorMessage = "Episode title is required")]
        [MaxLength(200, ErrorMessage = "Episode title cannot exceed 200 characters")]
        public string? EpisodeTitle { get; set; }

        [Required(ErrorMessage = "Episode description is required")]
        [MaxLength(1000, ErrorMessage = "Episode description cannot exceed 1000 characters")]
        public string? EpisodeDescription { get; set; }

        public string? AudioFile { get; set; }

        public TimeSpan EpisodeDuration { get; set; }
        public int EpisodeNumber { get; set; }
        public bool IsPublic {  get; set; }
        public string? Transcript { get; set; }
        public string? EpisodeCoverImg { get; set; }

        [ForeignKey("Podcast")]
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

        public ICollection<EpisodeLike>? EpisodeLikes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<EpisodeListener>? EpisodeListeners { get; set; }
    }
}
