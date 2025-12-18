namespace SocialMedia.Application;

public record UpdatePollCommand(Guid PollId, string Question, bool IsActive, DateTime? ExpiresAt, bool IsAnonymous, Guid GroupId) : ICommand<bool>;
