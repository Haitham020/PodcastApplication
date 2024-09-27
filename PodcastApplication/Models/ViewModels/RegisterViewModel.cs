using Microsoft.AspNetCore.Identity;
using PodcastApplication.Models.StaticData;
using System.ComponentModel.DataAnnotations;

namespace PodcastApplication.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Minimum Length is 8 characters")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password not match")]
        public string? ConfirmPassword { get; set; }
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int Age { get; set; }
        public string? Country { get; set; }

        public List<IdentityRole>? Roles { get; set; }

        public List<string> Countries { get; set; } = CountryList.GetCountries();

    }
}
