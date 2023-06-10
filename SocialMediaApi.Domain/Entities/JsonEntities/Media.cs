using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class Media
    {
        public PostMediaType MediaType { get; set; } = PostMediaType.TextOnly;
        public List<MediaContent> Content { get; set; } = new List<MediaContent>();
    }
}