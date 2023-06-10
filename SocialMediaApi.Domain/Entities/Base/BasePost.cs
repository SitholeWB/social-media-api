using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.Entities.Base
{
    public abstract class BasePost
    {
        public Guid Id { get; set; }
        public BaseUser Creator { get; set; } = new BaseUser();
        public EntityStatus EntityStatus { get; set; }
        public string? Text { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Downloads { get; set; } = 0;
        public int Views { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public Reaction Reactions { get; set; } = new Reaction { };
        public Media Media { get; set; } = new Media { };
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
        public DateTimeOffset ActionBasedDate { get; set; }
        public int Rank { get; set; }

        public static T GetChild<T>(BasePost post) where T : BasePost, new()
        {
            return new T
            {
                CreatedDate = post.CreatedDate,
                LastModifiedDate = post.LastModifiedDate,
                Creator = post.Creator,
                Downloads = post.Downloads,
                EntityStatus = post.EntityStatus,
                ActionBasedDate = post.ActionBasedDate,
                Id = post.Id,
                Media = post.Media,
                Rank = post.Rank
            };
        }
    }
}