
namespace SocialMedia.Domain;

public enum ReportStatus
{
    Pending,
    Reviewed,
    Dismissed
}

public class Report : BaseEntity
{
    public Guid ReporterId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    // Either PostId or CommentId will be set, not both
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }

    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
}
