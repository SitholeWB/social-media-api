using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.PostComments
{
    public class DeletePostCommentEvent : EventBase
    {
        public PostComment? PostComment { get; set; }
        public Post? Post { get; set; }
    }
}