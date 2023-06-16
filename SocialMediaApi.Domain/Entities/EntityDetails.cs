using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class EntityDetails
    {
        [Key]
        public Guid EntityId { get; set; }

        public ReactionSummary Summary { get; set; } = new ReactionSummary();
        public List<Reaction> Reactions { get; set; } = new List<Reaction>();
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}