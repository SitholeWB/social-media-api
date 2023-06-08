using SocialMediaApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.ActiveGroupPosts
{
    public class AddActiveGroupPostModel
    {
        [Required]
        public GroupPost GroupPost { get; set; } = default!;
    }
}