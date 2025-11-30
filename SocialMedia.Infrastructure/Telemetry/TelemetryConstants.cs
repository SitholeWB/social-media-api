namespace SocialMedia.Infrastructure;

public static class TelemetryConstants
{
    public const string ServiceName = "MediaService";
    public const string ServiceVersion = "1.0.0";

    // Activity Sources
    public const string DispatcherActivitySource = "MediaService.Dispatcher";
    public const string RepositoryActivitySource = "MediaService.Repository";

    // Meter Names
    public const string ApplicationMeter = "MediaService.Application";

    // Metric Names
    public const string CommandExecutionCount = "mediaservice.command.execution.count";
    public const string CommandExecutionDuration = "mediaservice.command.execution.duration";
    public const string QueryExecutionCount = "mediaservice.query.execution.count";
    public const string QueryExecutionDuration = "mediaservice.query.execution.duration";
    public const string RepositoryOperationCount = "mediaservice.repository.operation.count";
    public const string RepositoryOperationDuration = "mediaservice.repository.operation.duration";

    // Tag Names
    public const string CommandType = "command.type";
    public const string QueryType = "query.type";
    public const string OperationType = "operation.type";
    public const string EntityType = "entity.type";
    public const string Success = "success";
}
