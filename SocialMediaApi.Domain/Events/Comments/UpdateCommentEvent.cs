using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Comments
{
    public class UpdateCommentEvent
    {
        public Comment? Comment { get; set; }
        public Post? Post { get; set; }
    }
}