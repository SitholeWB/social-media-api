namespace SocialMedia.Domain;

public record CommentAddedEvent(Comment Comment) : IDomainEvent;