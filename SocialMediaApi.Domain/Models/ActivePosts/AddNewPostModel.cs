using SocialMediaApi.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Models.ActivePosts
{
    public class AddActivePostModel
    {
        [Required]
        public Post Post { get; set; } = default!;
    }
}