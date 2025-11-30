using SocialMedia.Domain;

namespace SocialMedia.Domain.Events;

public record LikeAddedEvent(Like Like) : IDomainEvent;
