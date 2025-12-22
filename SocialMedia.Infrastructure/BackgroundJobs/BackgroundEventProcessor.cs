namespace SocialMedia.Infrastructure;

public class BackgroundEventProcessor : IBackgroundEventProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundEventProcessor> _logger;
    private const int MaxRetries = 3;

    public BackgroundEventProcessor(
        IServiceProvider serviceProvider,
        ILogger<BackgroundEventProcessor> _logger)
    {
        _serviceProvider = serviceProvider;
        this._logger = _logger;
    }

    public async Task EnqueueEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = false,
            IncludeFields = true
        };

        var eventData = JsonSerializer.Serialize(@event, @event.GetType(), jsonOptions);
        var outboxEvent = new OutboxEvent
        {
            EventType = @event!.GetType().AssemblyQualifiedName ?? @event.GetType().FullName ?? @event.GetType().Name,
            EventData = eventData,
            Status = OutboxEventStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.OutboxEvents.Add(outboxEvent);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enqueued event {EventType} with ID {EventId}", typeof(TEvent).Name, outboxEvent.Id);
    }

    public async Task ProcessPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();

        var pendingEvents = await dbContext.OutboxEvents
            .Where(e => e.Status == OutboxEventStatus.Pending && e.RetryCount < MaxRetries)
            .OrderBy(e => e.CreatedAt)
            .Take(100) // Process in batches
            .ToListAsync(cancellationToken);

        var completedEvents = await dbContext.OutboxEvents
            .Where(e => e.Status == OutboxEventStatus.Completed)
            .OrderByDescending(e => e.CreatedAt)
            .Take(100) // Process in batches
            .ToListAsync(cancellationToken);
        if (completedEvents?.Count > 0)
        {
            _logger.LogInformation("START: Cleaning up {Count} completed outbox events", completedEvents.Count);
            dbContext.OutboxEvents.RemoveRange(completedEvents);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        foreach (var outboxEvent in pendingEvents)
        {
            await ProcessEventAsync(outboxEvent, dbContext, cancellationToken);
        }
    }

    private async Task ProcessEventAsync(OutboxEvent outboxEvent, SocialMediaDbContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            outboxEvent.Status = OutboxEventStatus.Processing;
            await dbContext.SaveChangesAsync(cancellationToken);

            // Deserialize event
            var eventType = Type.GetType(outboxEvent.EventType);
            if (eventType == null)
            {
                throw new InvalidOperationException($"Could not load event type: {outboxEvent.EventType}");
            }

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                IncludeFields = true
            };

            var @event = JsonSerializer.Deserialize(outboxEvent.EventData, eventType, jsonOptions);
            if (@event == null)
            {
                throw new InvalidOperationException($"Failed to deserialize event data for type: {outboxEvent.EventType}");
            }

            // Get all handlers for this event type
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            using var handlerScope = _serviceProvider.CreateScope();
            var handlers = handlerScope.ServiceProvider.GetServices(handlerType);

            // Execute all handlers
            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("Handle");
                if (handleMethod != null)
                {
                    var task = (Task?)handleMethod.Invoke(handler, new[] { @event, cancellationToken });
                    if (task != null)
                    {
                        await task;
                    }
                }
            }

            // Mark as completed
            outboxEvent.Status = OutboxEventStatus.Completed;
            outboxEvent.ProcessedAt = DateTime.UtcNow;
            outboxEvent.ErrorMessage = null;

            _logger.LogInformation("Successfully processed event {EventId} of type {EventType}", outboxEvent.Id, eventType.Name);
        }
        catch (Exception ex)
        {
            outboxEvent.RetryCount++;
            outboxEvent.LastRetryAt = DateTime.UtcNow;
            outboxEvent.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";

            if (outboxEvent.RetryCount >= MaxRetries)
            {
                outboxEvent.Status = OutboxEventStatus.Failed;
                _logger.LogError(ex, "Event {EventId} failed after {RetryCount} retries. Marking as Failed.", outboxEvent.Id, outboxEvent.RetryCount);
            }
            else
            {
                outboxEvent.Status = OutboxEventStatus.Pending;
                _logger.LogWarning(ex, "Event {EventId} failed. Retry {RetryCount}/{MaxRetries}", outboxEvent.Id, outboxEvent.RetryCount, MaxRetries);
            }
        }
        finally
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}