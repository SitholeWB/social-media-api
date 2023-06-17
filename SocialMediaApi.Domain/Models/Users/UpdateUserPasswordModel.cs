using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Users
{
    public class UpdateUserPasswordModel
    {
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}