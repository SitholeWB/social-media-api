namespace SocialMedia.Application;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public List<MediaDto> Media { get; set; } = [];
    public List<string> AdminTags { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}