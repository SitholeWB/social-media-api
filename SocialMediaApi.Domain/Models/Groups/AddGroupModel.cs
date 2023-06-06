using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Groups
{
    public class AddGroupModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}