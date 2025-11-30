using SocialMedia.Domain;

namespace SocialMedia.Domain.Events;

public record PostCreatedEvent(Post Post) : IDomainEvent;
