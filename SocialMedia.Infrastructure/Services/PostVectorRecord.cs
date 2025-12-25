namespace SocialMedia.Infrastructure;

/// <summary>
/// Record representing a post in the vector store
/// </summary>
public class PostVectorRecord
{
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    public required ReadOnlyMemory<float> Embedding { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
