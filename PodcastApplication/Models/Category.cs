using System.ComponentModel.DataAnnotations;

namespace PodcastApplication.Models
{
    public class Category : SharedProperties
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string? CategoryName { get; set; }
        public string? CategoryImg {  get; set; }

        public ICollection<Podcast>? Podcasts { get; set; }
    }
}
