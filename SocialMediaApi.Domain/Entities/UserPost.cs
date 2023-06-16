using SocialMediaApi.Domain.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class UserPost
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public bool IsFull { get; set; }
        public int Page { get; set; }
        public List<MiniEntity> Posts { get; set; } = new List<MiniEntity>();
        public DateTimeOffset CreatedDate { get; set; }
    }
}