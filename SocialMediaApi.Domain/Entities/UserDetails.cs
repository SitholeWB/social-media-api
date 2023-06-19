using SocialMediaApi.Domain.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class UserDetails
    {
        [Key]
        public Guid Id { get; set; }

        public List<MiniReaction> CommentReactions { get; set; } = new List<MiniReaction>();
        public List<MiniReaction> PostReactions { get; set; } = new List<MiniReaction>();
        public List<MiniEntity> Groups { get; set; } = new List<MiniEntity>();
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}