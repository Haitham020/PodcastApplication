using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PodcastApplication.Models
{
    public class PlaylistItem : SharedProperties
    {
        [Key]
        public int PlaylistItemId { get; set; }
        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }
        [ForeignKey("Podcast")]
        
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

    }
}
