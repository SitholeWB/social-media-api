using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Posts
{
    public class DeletePostEvent : EventBase
    {
        public Post? Post { get; set; }
    }
}