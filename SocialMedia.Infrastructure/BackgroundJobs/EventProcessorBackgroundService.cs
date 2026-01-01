namespace SocialMedia.Infrastructure;

public class EventProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventProcessorBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);
    private DateTimeOffset _lastRun = DateTimeOffset.MinValue;
    private bool _run = false;

    public EventProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EventProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastRun = DateTimeOffset.UtcNow;
        if (_run)
        {
            _logger.LogInformation("Event Processor Background Service is already running.");
            return;
        }
        _logger.LogInformation("Event Processor Background Service started");
        _run = true;
        while (!stoppingToken.IsCancellationRequested && _run)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventProcessor = scope.ServiceProvider.GetRequiredService<IBackgroundEventProcessor>();

                var count = await eventProcessor.ProcessPendingEventsAsync(stoppingToken);
                if (count > 0)
                {
                    _lastRun = DateTimeOffset.UtcNow;
                }
                else
                {
                    if (_lastRun.AddMinutes(10) < DateTimeOffset.UtcNow)
                    {
                        _logger.LogInformation("No events processed for 10 minutes, stopping background service.");
                        _run = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending events");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Event Processor Background Service stopped");
    }
}