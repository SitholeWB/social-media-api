using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Users
{
    public class UpdateUserModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string AboutMe { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}