using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodcastApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; } = false;
        public bool InActive { get; set; } = true;
        public string? Country {  get; set; }
        public DateTime DateJoined { get; set; }
        public string? ProfileImg { get; set; }
        
        [ForeignKey("Category")]
        public int? CreatorCategoryId { get; set; }
        public Category? CreatorCategory { get; set; }

    }

}
