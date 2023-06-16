using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Entities
{
    public class Post : BasePost
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; } = default!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}