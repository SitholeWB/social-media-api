namespace SocialMedia.Domain;

public enum OutboxEventStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public class OutboxEvent : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty; // JSON serialized event
    public OutboxEventStatus Status { get; set; } = OutboxEventStatus.Pending;
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? ErrorMessage { get; set; }
    public DateTime? LastRetryAt { get; set; }
}
