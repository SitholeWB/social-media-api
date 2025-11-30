
namespace SocialMedia.Application;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? FileId { get; set; }
}
