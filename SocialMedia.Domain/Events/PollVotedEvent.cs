namespace SocialMedia.Domain;

public record PollVotedEvent(Guid PollId, Guid OptionId, Guid UserId) : IDomainEvent;