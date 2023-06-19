using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.JwtTokens
{
    public class JwtTokenModel
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}