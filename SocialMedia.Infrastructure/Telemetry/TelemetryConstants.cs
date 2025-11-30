namespace SocialMedia.Infrastructure;

public static class TelemetryConstants
{
    public const string ServiceName = "SocialMedia";
    public const string ServiceVersion = "1.0.0";

    // Activity Sources
    public const string DispatcherActivitySource = "SocialMedia.Dispatcher";
    public const string RepositoryActivitySource = "SocialMedia.Repository";

    // Meter Names
    public const string ApplicationMeter = "SocialMedia.Application";

    // Metric Names
    public const string CommandExecutionCount = "socialmedia.command.execution.count";
    public const string CommandExecutionDuration = "socialmedia.command.execution.duration";
    public const string QueryExecutionCount = "socialmedia.query.execution.count";
    public const string QueryExecutionDuration = "socialmedia.query.execution.duration";
    public const string RepositoryOperationCount = "socialmedia.repository.operation.count";
    public const string RepositoryOperationDuration = "socialmedia.repository.operation.duration";

    // Tag Names
    public const string CommandType = "command.type";
    public const string QueryType = "query.type";
    public const string OperationType = "operation.type";
    public const string EntityType = "entity.type";
    public const string Success = "success";
}
