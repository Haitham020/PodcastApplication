namespace PodcastApplication.Models.ViewModels
{
    public class PodcastDetailViewModel
    {
        public Podcast? Podcast { get; set; }
        public List<Episode>? Episodes { get; set; }
        public int UserRatingValue { get; set; }
    }
}
