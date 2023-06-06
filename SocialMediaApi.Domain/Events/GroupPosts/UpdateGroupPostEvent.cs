using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPosts
{
    public class UpdateGroupPostEvent : EventBase
    {
        public GroupPost GroupPost { get; set; } = default!;
    }
}