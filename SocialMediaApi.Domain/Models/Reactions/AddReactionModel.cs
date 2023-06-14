using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Reactions
{
    public class AddReactionModel
    {
        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public string Unicode { get; set; } = string.Empty;
    }
}