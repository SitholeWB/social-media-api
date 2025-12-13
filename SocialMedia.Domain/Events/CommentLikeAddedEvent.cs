namespace SocialMedia.Domain;

public record CommentLikeAddedEvent(Like Like, ToggleLikeType ToggleLikeType) : IDomainEvent;