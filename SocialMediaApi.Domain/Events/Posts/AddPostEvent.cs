using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Posts
{
    public class AddPostEvent : EventBase
    {
        public Post? Post { get; set; }
    }
}