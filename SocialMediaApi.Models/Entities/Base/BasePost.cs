using SocialMediaApi.Models.Entities.JsonEntities;
using SocialMediaApi.Models.Enums;

namespace SocialMediaApi.Models.Entities.Base
{
    public abstract class BasePost
    {
        public Guid Id { get; set; }
        public BaseUser Creator { get; set; } = new BaseUser();
        public PostState PostState { get; set; }
        public string? Text { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Downloads { get; set; } = 0;
        public int Views { get; set; } = 0;
        public int TotalComments { get; set; } = 0;
        public Reaction Reactions { get; set; } = new Reaction { };
        public Media Media { get; set; } = new Media { };
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
    }
}