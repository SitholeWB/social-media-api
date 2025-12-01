namespace SocialMedia.Domain;

public record PostCreatedEvent(Post Post) : IDomainEvent;
