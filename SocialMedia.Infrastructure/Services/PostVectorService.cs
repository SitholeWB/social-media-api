namespace SocialMedia.Infrastructure;

/// <summary>
/// Service for managing post embeddings and vector-based recommendations
/// Uses ONNX-based embedding generator for semantic similarity
/// </summary>
public class PostVectorService : IPostVectorService
{
    private readonly SimpleInMemoryVectorStore _vectorStore;
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly ILogger<PostVectorService> _logger;

    public PostVectorService(
        SimpleInMemoryVectorStore vectorStore,
        IEmbeddingGenerator embeddingGenerator,
        ILogger<PostVectorService> logger)
    {
        _vectorStore = vectorStore;
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
    }

    public async Task<List<Guid>> GetRecommendedPostIdsAsync(
        Guid? userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // For demonstration: generate a query based on user preferences
            // In production, this could be based on user's interaction history
            var queryText = "interesting post"; // Placeholder - could be personalized
            var queryVector = await _embeddingGenerator.GenerateEmbeddingAsync(queryText, cancellationToken);

            var postIds = _vectorStore.Search(queryVector, count);

            return postIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommended posts");
            return new List<Guid>();
        }
    }

    public async Task UpsertPostEmbeddingAsync(
        Guid postId,
        string content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate real semantic embedding from content
            var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(content, cancellationToken);

            var record = new PostVectorRecord
            {
                PostId = postId,
                Content = content,
                Embedding = embedding,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _vectorStore.Upsert(record);
            _logger.LogInformation("Upserted embedding for post {PostId}", postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting post embedding for {PostId}", postId);
        }
    }

    public async Task RemovePostEmbeddingAsync(
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _vectorStore.Delete(postId);
            _logger.LogInformation("Removed embedding for post {PostId}", postId);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing post embedding for {PostId}", postId);
        }
    }
}
