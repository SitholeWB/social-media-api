using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;

namespace SocialMedia.Infrastructure;

/// <summary>
/// ONNX-based embedding generator using bge-micro-v2 model
/// This is a lightweight model (384 dimensions)
/// </summary>
public class OnnxEmbeddingGenerator : IEmbeddingGenerator, IDisposable
{
    private readonly InferenceSession _session;
    private readonly BertTokenizer _tokenizer;
    private readonly ILogger<OnnxEmbeddingGenerator> _logger;
    private const int MaxTokens = 512;

    public int Dimensions => 384;

    public OnnxEmbeddingGenerator(IConfiguration configuration, ILogger<OnnxEmbeddingGenerator> logger)
    {
        _logger = logger;

        try
        {
            var modelPath = configuration["EmbeddingSettings:ModelPath"] ?? "Models/bge-micro-v2/model.onnx";
            var vocabPath = configuration["EmbeddingSettings:VocabPath"] ?? "Models/bge-micro-v2/vocab.txt";

            if (!File.Exists(modelPath) || !File.Exists(vocabPath))
            {
                _logger.LogWarning("Embedding model or vocab not found at {ModelPath} / {VocabPath}. Attempting download...", modelPath, vocabPath);
                var modelsDirectory = Path.GetDirectoryName(modelPath) ?? "Models/bge-micro-v2";
                var (downloadedModelPath, downloadedVocabPath) = EmbeddingModelDownloader.EnsureModelsDownloadedAsync(modelsDirectory, _logger).GetAwaiter().GetResult();
                modelPath = downloadedModelPath;
                vocabPath = downloadedVocabPath;
            }

            _logger.LogInformation("Loading ONNX model from {Path}", modelPath);
            _session = new InferenceSession(modelPath);
            
            _logger.LogInformation("Loading Vocab from {Path}", vocabPath);
            using var vocabStream = File.OpenRead(vocabPath);
            _tokenizer = BertTokenizer.Create(vocabStream);
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
            return await Task.Run(() =>
            {
                // 1. Tokenize
                // Try EncodeToIds as Encode seems to be missing in this version's BertTokenizer
                var tokenIds = _tokenizer.EncodeToIds(text);
                var tokens = tokenIds.Select(id => (long)id).Take(MaxTokens).ToArray();
                var attentionMask = Enumerable.Repeat(1L, tokens.Length).ToArray();
                var typeIds = new long[tokens.Length]; 

                // 2. Prepare Tensors
                var dims = new int[] { 1, tokens.Length };
                var inputIdsTensor = new DenseTensor<long>(dims);
                var attentionMaskTensor = new DenseTensor<long>(dims);
                var typeIdsTensor = new DenseTensor<long>(dims);

                for (int i = 0; i < tokens.Length; i++)
                {
                    inputIdsTensor[0, i] = tokens[i];
                    attentionMaskTensor[0, i] = attentionMask[i];
                    typeIdsTensor[0, i] = 0;
                }

                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                    NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
                    NamedOnnxValue.CreateFromTensor("token_type_ids", typeIdsTensor)
                };

                // 3. Run Inference
                using var results = _session.Run(inputs);

                // 4. Post-processing (Mean Pooling)
                // bge-micro-v2 usually has 'last_hidden_state' as its main output
                // The output shape is [1, sequence_length, 384]
                var outputTensor = results.First(r => r.Name == "last_hidden_state").AsTensor<float>();
                return MeanPooling(outputTensor, attentionMask);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text.Length > 50 ? text.Substring(0, 50) : text);
            return CreateZeroVector();
        }
    }

    private ReadOnlyMemory<float> MeanPooling(Tensor<float> lastHiddenState, long[] attentionMask)
    {
        var batchSize = lastHiddenState.Dimensions[0]; // should be 1
        var seqLength = lastHiddenState.Dimensions[1];
        var hiddenSize = lastHiddenState.Dimensions[2];

        var pooledEmoji = new float[hiddenSize];
        float tokenCount = 0;

        for (int i = 0; i < seqLength; i++)
        {
            if (attentionMask[i] == 0) continue;

            tokenCount++;
            for (int j = 0; j < hiddenSize; j++)
            {
                pooledEmoji[j] += lastHiddenState[0, i, j];
            }
        }

        if (tokenCount > 0)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                pooledEmoji[j] /= tokenCount;
            }
        }

        // Normalize
        var magnitude = MathF.Sqrt(pooledEmoji.Sum(x => x * x));
        if (magnitude > 1e-6)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                pooledEmoji[j] /= magnitude;
            }
        }

        return new ReadOnlyMemory<float>(pooledEmoji);
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
