using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Entities
{
    public class Post : BasePost
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; } = default!;
        public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    }
}