using SocialMediaApi.Domain.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.GroupPosts
{
    public class AddGroupPostModel
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public Media Media { get; set; } = new Media { };
    }
}