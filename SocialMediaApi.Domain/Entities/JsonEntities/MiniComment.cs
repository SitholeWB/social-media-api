using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class MiniComment
    {
        [Required]
        public Guid Key { get; set; }

        [Required]
        public string Text { get; set; } = default!;

        public Reaction Reactions { get; set; } = new Reaction { };

        [Required]
        public Media Media { get; set; } = new Media { };
    }
}