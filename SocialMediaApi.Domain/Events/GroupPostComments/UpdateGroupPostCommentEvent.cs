using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.GroupPostComments
{
    public class UpdateGroupPostCommentEvent : EventBase
    {
        public GroupPostComment? GroupPostComment { get; set; }
        public GroupPost? GroupPost { get; set; }
    }
}