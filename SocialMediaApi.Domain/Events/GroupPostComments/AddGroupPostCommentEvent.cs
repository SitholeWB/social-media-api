using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPostComments
{
    public class AddGroupPostCommentEvent : EventBase
    {
        public GroupPostComment GroupPostComment { get; set; } = default!;
    }
}