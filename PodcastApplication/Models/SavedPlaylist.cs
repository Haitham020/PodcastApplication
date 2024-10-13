using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class SavedPlaylist
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }
    }
}
