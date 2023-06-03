using SocialMediaApi.Models.Entities.Base;

namespace SocialMediaApi.Models.Entities
{
    public class GroupPost : BasePost
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}