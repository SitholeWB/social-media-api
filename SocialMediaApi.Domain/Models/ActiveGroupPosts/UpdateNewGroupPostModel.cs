using SocialMediaApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.ActiveGroupPosts
{
    public class UpdateActiveGroupPostModel
    {
        [Required]
        public GroupPost GroupPost { get; set; } = default!;
    }
}