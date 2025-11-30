using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Domain;

namespace SocialMedia.IntegrationTests;

public static class TestHelpers
{
    public static async Task ProcessPendingEventsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IBackgroundEventProcessor>();

        while (await dbContext.OutboxEvents.AnyAsync(e => e.Status == OutboxEventStatus.Pending || e.Status == OutboxEventStatus.Processing))
        {
            // Only trigger processing if there are pending events. 
            // If events are Processing, they are being handled by another thread/process (or the previous call), so we just wait.
            if (await dbContext.OutboxEvents.AnyAsync(e => e.Status == OutboxEventStatus.Pending))
            {
                await eventProcessor.ProcessPendingEventsAsync();
            }
            else
            {
                // Wait for Processing events to complete
                await Task.Delay(50);
            }
        }

        var failedEvents = await dbContext.OutboxEvents
            .Where(e => e.Status == OutboxEventStatus.Failed)
            .ToListAsync();

        if (failedEvents.Any())
        {
            var errors = string.Join(", ", failedEvents.Select(e => $"{e.EventType}: {e.ErrorMessage}"));
            throw new Exception($"Events failed to process: {errors}");
        }
    }
}
