namespace SocialMedia.Application;

public record DeletePollCommand(Guid PollId) : ICommand<bool>;