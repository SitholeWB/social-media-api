using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class MiniReaction
    {
        public Guid EntityId { get; set; }

        [Required]
        public string Unicode { get; set; } = string.Empty;
    }
}