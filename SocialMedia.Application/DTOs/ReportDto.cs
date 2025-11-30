
namespace SocialMedia.Application;

public class ReportDto
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public DateTime CreatedAt { get; set; }
}
