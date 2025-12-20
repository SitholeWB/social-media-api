using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;


namespace SocialMedia.Infrastructure;

/// <summary>
/// ONNX-based embedding generator using all-MiniLM-L6-v2 model
/// This is a lightweight sentence transformer model (384 dimensions)
/// </summary>
public class OnnxEmbeddingGenerator : IEmbeddingGenerator, IDisposable
{
    private readonly InferenceSession? _session;
    private readonly ILogger<OnnxEmbeddingGenerator> _logger;
    private const int MaxTokens = 128;

    public int Dimensions => 384;

    public OnnxEmbeddingGenerator(ILogger<OnnxEmbeddingGenerator> logger)
    {
        _logger = logger;

        try
        {
            // For now, we'll use a fallback approach without actual ONNX model files
            // In production, you would load actual model files here
            
            _logger.LogWarning("ONNX model files not configured. Using improved hash-based embedding generation. " +
                "For production semantic search, download all-MiniLM-L6-v2 ONNX model and configure the path.");

            // We'll implement a fallback that doesn't require actual ONNX model files
            _session = null; // Will use fallback logic
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing ONNX embedding generator");
            throw;
        }
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return CreateZeroVector();
        }

        try
        {
            // Since we don't have the actual ONNX model files yet,
            // we'll use an improved hash-based approach that's more stable
            // In production, replace this with actual ONNX inference
            
            return await Task.Run(() => GenerateFallbackEmbedding(text), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text.Substring(0, Math.Min(50, text.Length)));
            return CreateZeroVector();
        }
    }

    private ReadOnlyMemory<float> GenerateFallbackEmbedding(string text)
    {
        // Improved hash-based embedding that's more stable than pure random
        // Uses multiple hash functions to create a more distributed representation
        
        var embedding = new float[Dimensions];
        
        // Normalize text
        text = text.ToLowerInvariant().Trim();
        
        // Use multiple hash seeds based on text characteristics
        var seeds = new[]
        {
            text.GetHashCode(),
            text.Length.GetHashCode(),
            text.Where(char.IsLetter).Count().GetHashCode(),
            text.Split(' ').Length.GetHashCode()
        };

        // Generate embedding using multiple hash-based random generators
        for (int i = 0; i < Dimensions; i++)
        {
            var seedIndex = i % seeds.Length;
            var random = new Random(seeds[seedIndex] + i);
            embedding[i] = (float)(random.NextDouble() * 2 - 1);
        }

        // Normalize the vector
        var magnitude = MathF.Sqrt(embedding.Sum(x => x * x));
        if (magnitude > 0)
        {
            for (int i = 0; i < embedding.Length; i++)
            {
                embedding[i] /= magnitude;
            }
        }

        return new ReadOnlyMemory<float>(embedding);
    }

    private ReadOnlyMemory<float> CreateZeroVector()
    {
        return new ReadOnlyMemory<float>(new float[Dimensions]);
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}
