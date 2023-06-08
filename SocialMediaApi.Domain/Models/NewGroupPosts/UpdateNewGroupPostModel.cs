using SocialMediaApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.NewGroupPosts
{
    public class UpdateNewGroupPostModel
    {
        [Required]
        public GroupPost GroupPost { get; set; } = default!;
    }
}