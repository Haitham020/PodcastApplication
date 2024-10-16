using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class UserEpisodeProgress
    {
        [Key]
        public int Id { get; set; }
        public TimeSpan Progress { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastUpdated { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Episode")]
        public Guid EpisodeId { get; set; }
        public Episode? Episode { get; set; }

    }
}
