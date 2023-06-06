using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPosts
{
    public class AddGroupPostEvent : EventBase
    {
        public GroupPost GroupPost { get; set; } = default!;
    }
}