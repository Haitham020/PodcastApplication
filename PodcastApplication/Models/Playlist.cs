using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Playlist : SharedProperties
    {
        [Key]
        public int PlaylistId { get; set; }
        [Required(ErrorMessage = "Enter Playlist Name")]
        public string? PlaylistName { get; set; }
        public string? PlaylistDescription { get; set; }
        public string? PlaylistImg {  get; set; }
        public bool IsPublic { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        
        public ICollection<PlaylistItem>? PlaylistItems { get; set; }
        public ICollection<SavedPlaylist>? SavedPlaylists { get; set; }
    }
}
