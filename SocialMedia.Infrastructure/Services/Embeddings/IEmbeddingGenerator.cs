namespace SocialMedia.Infrastructure;

/// <summary>
/// Interface for generating text embeddings
/// </summary>
public interface IEmbeddingGenerator
{
    /// <summary>
    /// Generate embedding vector for the given text
    /// </summary>
    /// <param name="text">Input text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Embedding vector</returns>
    Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dimension of the embedding vectors
    /// </summary>
    int Dimensions { get; }
}
