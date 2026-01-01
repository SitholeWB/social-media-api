namespace SocialMedia.Application;

public interface IBackgroundEventProcessor
{
    Task EnqueueEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;

    Task<int> ProcessPendingEventsAsync(CancellationToken cancellationToken = default);
}