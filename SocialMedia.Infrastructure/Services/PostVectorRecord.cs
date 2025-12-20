namespace SocialMedia.Infrastructure;

/// <summary>
/// Simple in-memory vector store implementation for demonstration
/// In production, replace with a real vector database (Qdrant, Milvus, etc.)
/// </summary>
public class SimpleInMemoryVectorStore
{
    private readonly Dictionary<Guid, PostVectorRecord> _records = new();
    private readonly object _lock = new();

    public void Upsert(PostVectorRecord record)
    {
        lock (_lock)
        {
            _records[record.PostId] = record;
        }
    }

    public void Delete(Guid postId)
    {
        lock (_lock)
        {
            _records.Remove(postId);
        }
    }

    public List<Guid> Search(ReadOnlyMemory<float> queryVector, int topK)
    {
        lock (_lock)
        {
            if (_records.Count == 0)
            {
                return new List<Guid>();
            }

            // Calculate cosine similarity for each record
            var results = _records.Values
                .Select(record => new
                {
                    PostId = record.PostId,
                    Similarity = CosineSimilarity(queryVector.Span, record.Embedding.Span)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .Select(x => x.PostId)
                .ToList();

            return results;
        }
    }

    private static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException("Vectors must have the same length");
        }

        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0f;
        }

        return dotProduct / (MathF.Sqrt(magnitudeA) * MathF.Sqrt(magnitudeB));
    }
}

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
