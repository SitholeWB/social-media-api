namespace SocialMedia.Infrastructure;

/// <summary>
/// Record representing a post in the vector store
/// </summary>
public class PostVectorRecord
{
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public ReadOnlyMemory<float> Embedding { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}