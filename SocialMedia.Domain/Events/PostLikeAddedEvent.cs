namespace SocialMedia.Domain;

public record PostLikeAddedEvent(Like Like, ToggleLikeType ToggleLikeType, string OldEmoji) : IDomainEvent;