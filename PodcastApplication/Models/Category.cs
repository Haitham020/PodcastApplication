namespace PodcastApplication.Models
{
    public class Category : SharedProperties
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryImg {  get; set; }

        public ICollection<Podcast>? Podcasts { get; set; }
    }
}
