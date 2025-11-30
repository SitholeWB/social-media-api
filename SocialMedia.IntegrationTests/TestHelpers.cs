using Microsoft.Extensions.DependencyInjection;

namespace SocialMedia.IntegrationTests;

public static class TestHelpers
{
    public static async Task ProcessPendingEventsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IBackgroundEventProcessor>();
        await eventProcessor.ProcessPendingEventsAsync();
    }
}
