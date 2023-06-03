using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public BaseUser Creator { get; set; } = new BaseUser();
        public PostState PostState { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }

        public virtual ICollection<GroupPost> Posts { get; set; }
    }
}