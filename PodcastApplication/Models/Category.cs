﻿namespace PodcastApplication.Models
{
    public class Category : SharedProperties
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public ICollection<ApplicationUser>? Creators { get; set; }
    }
}
