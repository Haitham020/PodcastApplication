using PodcastApplication.Models;

namespace PodcastApplication.Areas.Administrator.Models.ViewModels
{
    public class UsersViewModel
    {
        public ApplicationUser? User { get; set; }
        public string? Role { get; set; }
    }
}
