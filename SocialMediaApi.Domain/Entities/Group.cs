using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string Description { get; set; } = string.Empty;
        public BaseUser Creator { get; set; } = new BaseUser();
        public EntityStatus EntityStatus { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}