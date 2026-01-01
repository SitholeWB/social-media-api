namespace SocialMedia.Infrastructure;

// VectorData property mapping for future extensibility as requested
public class PostVectorData
{
    public Guid PostId { get; set; }

    public string Content { get; set; } = string.Empty;

    public ReadOnlyMemory<float> Embedding { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}