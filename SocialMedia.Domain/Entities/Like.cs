
namespace SocialMedia.Domain;

public class Like : BaseEntity
{
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
    public Guid UserId { get; set; }
    public string Emoji { get; set; } = "👍";
}
