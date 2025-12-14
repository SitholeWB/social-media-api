namespace SocialMedia.Domain;

public record CommentDeletedEvent(Comment Comment) : IDomainEvent;