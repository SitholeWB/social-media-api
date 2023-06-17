using SocialMediaApi.Domain.Entities.JsonEntities;

namespace SocialMediaApi.Domain.Entities
{
    public class UserPost
    {
        public string Id { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public bool IsFull { get; set; }
        public int Page { get; set; }
        public List<MiniEntity> Posts { get; set; } = new List<MiniEntity>();
        public DateTimeOffset CreatedDate { get; set; }
    }
}