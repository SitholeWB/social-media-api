namespace SocialMedia.IntegrationTests;

public static class TestHelpers
{
    public static async Task ProcessPendingEventsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IBackgroundEventProcessor>();

        while (await dbContext.OutboxEvents.AnyAsync(e => e.Status == OutboxEventStatus.Pending || e.Status == OutboxEventStatus.Processing, cancellationToken))
        {
            // Only trigger processing if there are pending events. If events are Processing, they
            // are being handled by another thread/process (or the previous call), so we just wait.
            if (await dbContext.OutboxEvents.AnyAsync(e => e.Status == OutboxEventStatus.Pending, cancellationToken))
            {
                await eventProcessor.ProcessPendingEventsAsync(cancellationToken);
            }
            else
            {
                // Wait for Processing events to complete
                await Task.Delay(50, cancellationToken);
            }
        }

        var failedEvents = await dbContext.OutboxEvents
            .Where(e => e.Status == OutboxEventStatus.Failed)
            .ToListAsync(cancellationToken);

        if (failedEvents.Any())
        {
            var errors = string.Join(", ", failedEvents.Select(e => $"{e.EventType}: {e.ErrorMessage}"));
            throw new Exception($"Events failed to process: {errors}");
        }
    }
}