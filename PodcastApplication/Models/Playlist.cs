using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public string? PlaylistName { get; set; }
        public string? PlaylistDescription { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        
        public ICollection<PlaylistItem>? PlaylistItems { get; set; }
    }
}
