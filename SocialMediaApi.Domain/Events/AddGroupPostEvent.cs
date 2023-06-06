using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class AddGroupPostEvent : EventBase
    {
        public GroupPost GroupPost { get; set; } = default!;
    }
}