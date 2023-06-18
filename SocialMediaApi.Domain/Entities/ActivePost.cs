using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Entities
{
    public class ActivePost : BasePost
    {
        public Guid OwnerId { get; set; }
        //public List<MiniComment> Comments { get; set; } = new List<MiniComment>();
    }
}