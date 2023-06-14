using SocialMediaApi.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.ViewModels
{
    public class ReactionViewModel
    {
        public Guid EntityId { get; set; }

        [Required]
        public string Unicode { get; set; } = string.Empty;

        public BaseUser Creator { get; set; } = new BaseUser();
    }
}