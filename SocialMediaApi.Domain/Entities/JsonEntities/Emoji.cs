using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class Emoji
    {
        [Required]
        public string Unicode { get; set; } = string.Empty;

        public int Count { get; set; } = 1;
    }
}