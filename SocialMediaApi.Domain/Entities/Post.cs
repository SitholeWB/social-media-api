using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Entities
{
    public class Post : BasePost
    {
        public Guid OwnerId { get; set; }
    }
}