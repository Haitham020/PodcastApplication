using System.ComponentModel.DataAnnotations;

namespace PodcastApplication.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }
        public string? MenuTitle { get; set; }
        public string? ActionUrl { get; set; }
        public string? ControllerUrl { get; set; }
        public int ParentMenuId { get; set; }
    }
}
