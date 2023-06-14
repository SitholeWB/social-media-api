using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPosts
{
    public class DeleteGroupPostEvent : EventBase
    {
        public GroupPost? GroupPost { get; set; }
    }
}