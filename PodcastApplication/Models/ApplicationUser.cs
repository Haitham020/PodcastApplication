using Microsoft.AspNetCore.Identity;

namespace PodcastApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; } = false;
        public bool InActive { get; set; } = true;
        public string? Country {  get; set; }
        public DateTime DateJoined { get; set; }
        public string? ProfileImg { get; set; }
        public string? ProfileBio { get; set; }
        public int Age { get; set; }
        public string? CreatorGenre { get; set; }

        public ICollection<Podcast>? Podcasts { get; set; }
    }

}
