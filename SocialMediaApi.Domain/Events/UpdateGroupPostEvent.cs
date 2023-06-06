using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class UpdateGroupPostEvent : EventBase
    {
        public GroupPost GroupPost { get; set; } = default!;
    }
}