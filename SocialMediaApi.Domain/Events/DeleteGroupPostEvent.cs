using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class DeleteGroupPostEvent : EventBase
    {
        public GroupPost GroupPost { get; set; } = default!;
    }
}