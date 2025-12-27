namespace SocialMedia.Application;

public record ReportCommentCommand(Guid CommentId, Guid ReporterId) : ICommand<Guid>
{
    public string Reason { get; set; } = string.Empty;
}