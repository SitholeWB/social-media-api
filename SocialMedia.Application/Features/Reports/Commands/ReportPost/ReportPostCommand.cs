namespace SocialMedia.Application;

public record ReportPostCommand(Guid PostId, Guid ReporterId) : ICommand<Guid>
{
    public string Reason { get; set; } = string.Empty;
}
