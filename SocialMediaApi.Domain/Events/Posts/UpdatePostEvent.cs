using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Posts
{
    public class UpdatePostEvent
    {
        public Post? Post { get; set; }
    }
}