using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Posts
{
    public class AddPostEvent
    {
        public Post? Post { get; set; }
    }
}