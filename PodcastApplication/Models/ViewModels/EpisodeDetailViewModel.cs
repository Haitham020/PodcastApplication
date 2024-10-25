namespace PodcastApplication.Models.ViewModels
{
    public class EpisodeDetailViewModel
    {
        public Episode? Episode { get; set; }
        public List<Episode>? RelatedEpisodes { get; set; }
        public int EpisodeNumber { get; set; }
    }
}
