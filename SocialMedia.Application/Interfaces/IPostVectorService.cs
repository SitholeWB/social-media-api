namespace SocialMedia.Application;

/// <summary>
/// Service for managing post embeddings and vector-based recommendations
/// </summary>
public interface IPostVectorService
{
    /// <summary>
    /// Get recommended posts based on vector similarity
    /// </summary>
    Task<List<Guid>> GetRecommendedPostIdsAsync(
        Guid? userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update or create embedding for a post
    /// </summary>
    Task UpsertPostEmbeddingAsync(
        Guid postId,
        string content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove post embedding
    /// </summary>
    Task RemovePostEmbeddingAsync(
        Guid postId,
        CancellationToken cancellationToken = default);
}