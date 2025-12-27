namespace SocialMedia.Infrastructure;

/// <summary>
/// Service for managing post embeddings and vector-based recommendations Uses ONNX-based embedding
/// generator for semantic similarity
/// </summary>
public class PostVectorService : IPostVectorService
{
    private readonly SqliteVectorStore _vectorStore;
    private readonly IEmbeddingGenerator _embeddingGenerator;
    private readonly ILogger<PostVectorService> _logger;

    public PostVectorService(
        SqliteVectorStore vectorStore,
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
            float[] queryVector;

            // 1. Try to build a personalized query vector using interaction history
            if (userId.HasValue)
            {
                var historyEmbeddings = await _vectorStore.GetUserInteractionEmbeddingsAsync(userId.Value, limit: 5);
                if (historyEmbeddings.Count > 0)
                {
                    // Calculate average vector (centroid) of interacted posts Simple average: sum
                    // each dimension, divide by count
                    int dim = historyEmbeddings[0].Length;
                    var sumVector = new float[dim];
                    foreach (var vec in historyEmbeddings)
                    {
                        for (int i = 0; i < dim; i++) sumVector[i] += vec[i];
                    }
                    for (int i = 0; i < dim; i++) sumVector[i] /= historyEmbeddings.Count;

                    queryVector = sumVector;
                    _logger.LogInformation("Using personalized query vector for user {UserId} based on {Count} interactions.", userId, historyEmbeddings.Count);
                }
                else
                {
                    // No history, fallback to generic "interesting" query or global trends
                    _logger.LogInformation("No interaction history for user {UserId}. Using default query.", userId);
                    var queryText = "globally interesting trending content";
                    var roVector = await _embeddingGenerator.GenerateEmbeddingAsync(queryText, cancellationToken);
                    queryVector = roVector.ToArray();
                }
            }
            else
            {
                // Anonymous user
                var queryText = "popular general content";
                var roVector = await _embeddingGenerator.GenerateEmbeddingAsync(queryText, cancellationToken);
                queryVector = roVector.ToArray();
            }

            // 2. Perform search
            var postIds = await _vectorStore.SearchAsync(new ReadOnlyMemory<float>(queryVector), count);

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

            await _vectorStore.UpsertAsync(record);
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
            await _vectorStore.DeleteAsync(postId);
            _logger.LogInformation("Removed embedding for post {PostId}", postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing post embedding for {PostId}", postId);
        }
    }

    public async Task RecordInteractionAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("User {UserId} interacted with post {PostId}. Recording interaction.", userId, postId);

            // Store the interaction permanently
            await _vectorStore.RecordUserInteractionAsync(userId, postId);

            _logger.LogDebug("Interaction recorded successfully.");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording interaction for user {UserId} and post {PostId}", userId, postId);
        }
    }
}