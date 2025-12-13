namespace SocialMedia.Domain;

public record PostLikeAddedEvent(Like Like, ToggleLikeType ToggleLikeType) : IDomainEvent;