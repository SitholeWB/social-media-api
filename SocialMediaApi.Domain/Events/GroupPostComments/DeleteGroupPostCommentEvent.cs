using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPostComments
{
    public class DeleteGroupPostCommentEvent : EventBase
    {
        public GroupPostComment GroupPostComment { get; set; } = default!;
    }
}