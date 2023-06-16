using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public BaseUser Creator { get; set; } = new BaseUser();
        public EntityStatus EntityStatus { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Views { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public ReactionSummary Reactions { get; set; } = new ReactionSummary { };
        public Media Media { get; set; } = new Media { };
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
        public DateTimeOffset ActionBasedDate { get; set; }
    }
}