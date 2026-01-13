namespace SocialMedia.Application;

public record SubmitFeedbackCommand(SubmitFeedbackRequest Request) : ICommand<bool>;
