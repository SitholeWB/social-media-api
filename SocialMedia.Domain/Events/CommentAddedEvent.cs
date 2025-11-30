using SocialMedia.Domain;

namespace SocialMedia.Domain.Events;

public record CommentAddedEvent(Comment Comment) : IDomainEvent;
