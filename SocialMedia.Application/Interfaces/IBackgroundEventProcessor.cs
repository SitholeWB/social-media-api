using SocialMedia.Domain.Events;

namespace SocialMedia.Application;

public interface IBackgroundEventProcessor
{
    Task EnqueueEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    Task ProcessPendingEventsAsync(CancellationToken cancellationToken = default);
}
