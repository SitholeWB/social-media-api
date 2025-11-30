
namespace SocialMedia.Application;

public class LikeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public string Emoji { get; set; } = string.Empty;
}
