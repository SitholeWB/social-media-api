using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Reactions
{
    public class AddReactionModel
    {
        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public string Unicode { get; set; } = string.Empty;
    }
}