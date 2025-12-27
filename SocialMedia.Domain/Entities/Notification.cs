namespace SocialMedia.Domain;

public enum NotificationType
{
    Comment,
    LikePost,
    LikeComment,
    System
}

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Guid? RelatedId { get; set; } // PostId, CommentId, etc.
    public bool IsRead { get; set; }
}