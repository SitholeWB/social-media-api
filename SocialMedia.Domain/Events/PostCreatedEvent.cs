namespace SocialMedia.Domain;

public record PostDeletedEvent(Post Post) : IDomainEvent;