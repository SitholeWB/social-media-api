using SocialMediaApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities.JsonEntities
{
    public class Media
    {
        public PostMediaType MediaType { get; set; } = PostMediaType.TextOnly;
        public IList<Content> Content { get; set; } = new List<Content>();
    }

    public class Content
    {
        public Guid Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}