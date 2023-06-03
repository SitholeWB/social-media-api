using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.Group
{
    public class AddGroupModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}