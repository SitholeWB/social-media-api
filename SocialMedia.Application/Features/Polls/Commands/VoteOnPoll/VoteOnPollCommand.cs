namespace SocialMedia.Application;

public record VoteOnPollCommand(Guid PollId, Guid PollOptionId, Guid UserId) : ICommand<bool>;