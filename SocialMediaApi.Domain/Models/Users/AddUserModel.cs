using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Users
{
    public class AddUserModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        public string AboutMe { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}