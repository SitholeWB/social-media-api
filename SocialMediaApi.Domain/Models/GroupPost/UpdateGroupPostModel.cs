using SocialMediaApi.Domain.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.GroupPost
{
    public class UpdateGroupPostModel
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        [Required]
        public Media Media { get; set; } = new Media { };
    }
}