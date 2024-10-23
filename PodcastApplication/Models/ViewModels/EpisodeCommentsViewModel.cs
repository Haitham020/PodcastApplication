namespace PodcastApplication.Models.ViewModels
{
    public class EpisodeCommentsViewModel
    {
        public Episode? Episode { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
