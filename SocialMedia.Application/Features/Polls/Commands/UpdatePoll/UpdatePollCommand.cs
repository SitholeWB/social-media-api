namespace SocialMedia.Application;

public record UpdatePollCommand(Guid PollId, string Question, bool IsActive, DateTime? ExpiresAt) : ICommand<bool>;
