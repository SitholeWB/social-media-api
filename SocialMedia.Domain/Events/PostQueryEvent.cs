namespace SocialMedia.Domain;

public record PostQueryEvent(Guid UserId) : IDomainEvent;