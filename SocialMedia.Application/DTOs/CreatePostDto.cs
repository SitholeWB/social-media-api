namespace SocialMedia.Application;

public class CreatePostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public Guid AuthorId { get; set; }
    public string? FileUrl { get; set; }
    public bool StatusFullScreen { get; set; } = false;
    public List<string> AdminTags { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}