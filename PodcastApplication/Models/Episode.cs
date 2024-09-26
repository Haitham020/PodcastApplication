﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class Episode : SharedProperties
    {
        public Guid EpisodeId { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? EpisodeDescription { get; set; }
        public string? AudioFile { get; set; }
        public TimeSpan EpisodeDuration { get; set; }

        [ForeignKey("Podcast")]
        public Guid PodcastId { get; set; }
        public Podcast? Podcast { get; set; }

        public ICollection<EpisodeLike>? EpisodeLikes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }

    }
}