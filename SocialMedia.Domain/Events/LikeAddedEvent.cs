namespace SocialMedia.Domain;

public record LikeAddedEvent(Like Like) : IDomainEvent;
