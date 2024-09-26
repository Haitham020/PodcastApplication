using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class PlaylistItem
    {
        [Key]
        public int PlaylistItemId { get; set; }
        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }
        [ForeignKey("Podcast")]
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

        [ForeignKey("Episode")]
        public Guid EpisodeId { get; set; }
        public Episode? Episode { get; set; }

    }
}
