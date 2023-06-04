using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.ViewModels
{
    public class GroupViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BaseUser Creator { get; set; } = new BaseUser();
        public EntityStatus PostState { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}