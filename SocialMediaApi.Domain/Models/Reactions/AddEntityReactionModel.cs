using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Reactions
{
    public class AddEntityReactionModel
    {
        [Required]
        public string Unicode { get; set; } = string.Empty;
    }
}