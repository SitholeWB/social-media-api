namespace SocialMedia.Domain;

public record CommentLikeAddedEvent(Like Like, ToggleLikeType ToggleLikeType, string OldEmoji) : IDomainEvent;