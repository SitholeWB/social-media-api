namespace SocialMedia.Application;

public class SubmitFeedbackRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
