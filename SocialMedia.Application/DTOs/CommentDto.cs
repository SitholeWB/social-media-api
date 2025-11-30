
namespace SocialMedia.Application;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
