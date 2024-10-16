namespace PodcastApplication.Models.ViewModels
{
    public class SearchViewModel
    {
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Podcast>? Podcasts { get; set; }
        public ICollection<Episode>? Episodes { get; set; }
    }
}
