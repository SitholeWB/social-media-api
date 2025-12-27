using Microsoft.ML.Tokenizers;
using Tensorflow;
using static Tensorflow.Binding;

namespace SocialMedia.Infrastructure;

/// <summary>
/// TensorFlow-based embedding generator using TinyBERT (L-2, H-128) This is an extremely
/// lightweight model (128 dimensions) optimized for shared hosting.
/// </summary>
public class TensorFlowEmbeddingGenerator : IEmbeddingGenerator, IDisposable
{
    private Graph? _graph;
    private Session? _session;
    private readonly BertTokenizer _tokenizer;
    private readonly ILogger<TensorFlowEmbeddingGenerator> _logger;
    private const int MaxTokens = 128;

    public int Dimensions => 128;

    public TensorFlowEmbeddingGenerator(IConfiguration configuration, ILogger<TensorFlowEmbeddingGenerator> logger)
    {
        _logger = logger;

        try
        {
            var modelsDirectory = Path.Combine(AppContext.BaseDirectory, "Models", "TinyBERT");
            var modelDir = configuration["EmbeddingSettings:ModelPath"] ?? Path.Combine(modelsDirectory, "saved_model");
            var vocabPath = configuration["EmbeddingSettings:VocabPath"] ?? Path.Combine(modelsDirectory, "vocab.txt");

            if (!Directory.Exists(modelDir) || !File.Exists(vocabPath))
            {
                _logger.LogWarning("Embedding model directory or vocab not found at {ModelDir}. Attempting download...", modelDir);
                var (downloadedModelPath, downloadedVocabPath) = EmbeddingModelDownloader.EnsureModelsDownloadedAsync(modelsDirectory, _logger).GetAwaiter().GetResult();
            }

            _logger.LogInformation("Loading TensorFlow SavedModel from {Dir}", modelDir);

            try
            {
                // API Discovery confirmed 'LoadFromSavedModel' is a static method on Session
                _session = Session.LoadFromSavedModel(modelDir);
                _graph = _session.graph;
                _logger.LogInformation("Successfully loaded SavedModel using Session.LoadFromSavedModel.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Session.LoadFromSavedModel failed: {Msg}. Trying legacy Import.", ex.Message);
                _graph = tf.Graph().as_default();
                _session = tf.Session(_graph);

                var pbPath = Path.Combine(modelDir, "saved_model.pb");
                if (File.Exists(pbPath))
                {
                    _graph.Import(pbPath);
                }
            }

            // Diagnostic: Log available operations
            if (_graph != null)
            {
                var ops = _graph.get_operations();
                _logger.LogInformation("Model Operations ({Count}) Total. First 30: {Ops}", ops.Length, string.Join(", ", ops.Take(30).Select(o => o.name)));
            }

            _logger.LogInformation("Loading Vocab from {Path}", vocabPath);
            using var vocabStream = File.OpenRead(vocabPath);
            _tokenizer = BertTokenizer.Create(vocabStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing TensorFlow embedding generator");
            _tokenizer = null!;
        }
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text) || _session == null || _graph == null)
        {
            return CreateZeroVector();
        }

        try
        {
            return await Task.Run(() =>
            {
                var tokenIds = _tokenizer.EncodeToIds(text);
                var tokens = tokenIds.Select(id => (int)id).Take(MaxTokens).ToArray();

                var inputIdsTensor = tf.constant(tokens, shape: new long[] { 1, tokens.Length });

                // Robust lookup: Search for operations that end with the expected names
                var ops = _graph.get_operations();
                var inputOp = ops.FirstOrDefault(o => o.name.EndsWith("input_word_ids") || o.name.EndsWith("input_ids"));
                var outputOp = ops.FirstOrDefault(o => o.name.EndsWith("pooled_output") || o.name == "StatefulPartitionedCall" || o.name.EndsWith("last_hidden_state/Reshape"));

                if (inputOp == null || outputOp == null)
                {
                    _logger.LogWarning("Required TensorFlow operations not found. Available ops: {Ops}",
                        string.Join(", ", ops.Take(5).Select(o => o.name)));
                    return CreateZeroVector();
                }

                var results = _session.run(new Operation[] { outputOp.op }, new FeedItem[] { new FeedItem(inputOp, inputIdsTensor) });

                if (results == null || results.Length == 0)
                {
                    return CreateZeroVector();
                }

                var outputArray = results[0].ToArray<float>();
                return Normalize(outputArray);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding");
            return CreateZeroVector();
        }
    }

    private ReadOnlyMemory<float> Normalize(float[] vector)
    {
        var magnitude = MathF.Sqrt(vector.Sum(x => x * x));
        if (magnitude > 1e-6)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] /= magnitude;
            }
        }
        return new ReadOnlyMemory<float>(vector);
    }

    private ReadOnlyMemory<float> CreateZeroVector() => new ReadOnlyMemory<float>(new float[Dimensions]);

    public void Dispose()
    {
        _session?.Dispose();
    }
}