using SocialMediaApi.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.JsonEntities
{
    public class Reaction
    {
        [Required]
        public string Unicode { get; set; } = string.Empty;

        public BaseUser Creator { get; set; } = new BaseUser();
    }
}