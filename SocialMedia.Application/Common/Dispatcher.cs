using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Application.Common.Validation;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using SocialMedia.Domain.Events;

namespace SocialMedia.Application;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ActivitySource ActivitySource = new(TelemetryConstants.DispatcherActivitySource);
    private static readonly Meter Meter = new(TelemetryConstants.ApplicationMeter);
    private static readonly Counter<long> CommandCounter = Meter.CreateCounter<long>(TelemetryConstants.CommandExecutionCount);
    private static readonly Counter<long> QueryCounter = Meter.CreateCounter<long>(TelemetryConstants.QueryExecutionCount);
    private static readonly Histogram<double> CommandDuration = Meter.CreateHistogram<double>(TelemetryConstants.CommandExecutionDuration, "ms");
    private static readonly Histogram<double> QueryDuration = Meter.CreateHistogram<double>(TelemetryConstants.QueryExecutionDuration, "ms");

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>
    {
        using var activity = ActivitySource.StartActivity($"Command: {typeof(TCommand).Name}", ActivityKind.Internal);
        activity?.SetTag(TelemetryConstants.CommandType, typeof(TCommand).Name);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Validation
            var validators = _serviceProvider.GetServices<IValidator<TCommand>>();
            if (validators.Any())
            {
                var validationErrors = new List<ValidationError>();
                foreach (var validator in validators)
                {
                    var validationResult = await validator.ValidateAsync(command, cancellationToken);
                    if (!validationResult.IsValid)
                    {
                        validationErrors.AddRange(validationResult.Errors);
                    }
                }

                if (validationErrors.Count != 0)
                {
                    throw new ValidationException(validationErrors);
                }
            }

            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            var result = await handler.Handle(command, cancellationToken);

            stopwatch.Stop();
            CommandCounter.Add(1, new KeyValuePair<string, object?>(TelemetryConstants.CommandType, typeof(TCommand).Name), new KeyValuePair<string, object?>(TelemetryConstants.Success, "true"));
            CommandDuration.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>(TelemetryConstants.CommandType, typeof(TCommand).Name));
            activity?.SetTag(TelemetryConstants.Success, true);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            CommandCounter.Add(1, new KeyValuePair<string, object?>(TelemetryConstants.CommandType, typeof(TCommand).Name), new KeyValuePair<string, object?>(TelemetryConstants.Success, "false"));
            CommandDuration.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>(TelemetryConstants.CommandType, typeof(TCommand).Name));
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag(TelemetryConstants.Success, false);
            activity?.SetTag(TelemetryConstants.ExceptionType, ex.GetType().FullName);
            activity?.SetTag(TelemetryConstants.ExceptionMessage, ex.Message);
            throw;
        }
    }

    public async Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
    {
        using var activity = ActivitySource.StartActivity($"Query: {typeof(TQuery).Name}", ActivityKind.Internal);
        activity?.SetTag(TelemetryConstants.QueryType, typeof(TQuery).Name);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            var result = await handler.Handle(query, cancellationToken);

            stopwatch.Stop();
            QueryCounter.Add(1, new KeyValuePair<string, object?>(TelemetryConstants.QueryType, typeof(TQuery).Name), new KeyValuePair<string, object?>(TelemetryConstants.Success, "true"));
            QueryDuration.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>(TelemetryConstants.QueryType, typeof(TQuery).Name));
            activity?.SetTag(TelemetryConstants.Success, true);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            QueryCounter.Add(1, new KeyValuePair<string, object?>(TelemetryConstants.QueryType, typeof(TQuery).Name), new KeyValuePair<string, object?>(TelemetryConstants.Success, "false"));
            QueryDuration.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>(TelemetryConstants.QueryType, typeof(TQuery).Name));
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag(TelemetryConstants.Success, false);
            activity?.SetTag(TelemetryConstants.ExceptionType, ex.GetType().FullName);
            activity?.SetTag(TelemetryConstants.ExceptionMessage, ex.Message);
            throw;
        }
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        using var activity = ActivitySource.StartActivity($"Event: {typeof(TEvent).Name}", ActivityKind.Internal);
        activity?.SetTag(TelemetryConstants.EventType, typeof(TEvent).Name);

        var eventProcessor = _serviceProvider.GetRequiredService<IBackgroundEventProcessor>();
        await eventProcessor.EnqueueEventAsync(@event, cancellationToken);
        
        activity?.SetTag(TelemetryConstants.Success, true);
    }
}
