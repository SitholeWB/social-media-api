using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class MiniComment
    {
        [Required]
        public Guid Key { get; set; }

        [Required]
        public string Text { get; set; } = default!;

        public ReactionSummary Reactions { get; set; } = new ReactionSummary { };

        [Required]
        public Media Media { get; set; } = new Media { };
    }
}