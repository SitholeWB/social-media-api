using SocialMediaApi.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class Reaction
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public string Unicode { get; set; } = string.Empty;

        public BaseUser Creator { get; set; } = new BaseUser();
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}