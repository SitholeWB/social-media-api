using MediatR;

namespace SocialMedia.Domain.Events;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent> 
    where TEvent : IDomainEvent
{
}
