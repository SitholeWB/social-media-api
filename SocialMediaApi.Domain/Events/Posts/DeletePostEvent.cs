using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Posts
{
    public class DeletePostEvent
    {
        public Post? Post { get; set; }
    }
}