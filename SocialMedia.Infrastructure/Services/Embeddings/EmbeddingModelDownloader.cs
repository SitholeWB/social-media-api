namespace SocialMedia.Infrastructure;

/// <summary>
/// Utility to download ONNX embedding model files
/// </summary>
public static class EmbeddingModelDownloader
{
    private const string ModelUrl = "https://huggingface.co/TaylorAI/bge-micro-v2/resolve/main/onnx/model.onnx";
    private const string VocabUrl = "https://huggingface.co/TaylorAI/bge-micro-v2/resolve/main/vocab.txt";

    public static async Task<(string modelPath, string vocabPath)> EnsureModelsDownloadedAsync(
        string modelsDirectory,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(modelsDirectory);

        var modelPath = Path.Combine(modelsDirectory, "bge-micro-v2.onnx");
        var vocabPath = Path.Combine(modelsDirectory, "vocab.txt");

        // Download model if not exists
        if (!File.Exists(modelPath))
        {
            logger.LogInformation("Downloading ONNX model from {Url}...", ModelUrl);
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            var modelBytes = await httpClient.GetByteArrayAsync(ModelUrl, cancellationToken);
            await File.WriteAllBytesAsync(modelPath, modelBytes, cancellationToken);
            logger.LogInformation("Model downloaded successfully to {Path}", modelPath);
        }

        // Download vocab if not exists
        if (!File.Exists(vocabPath))
        {
            logger.LogInformation("Downloading vocab from {Url}...", VocabUrl);
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            var vocabBytes = await httpClient.GetByteArrayAsync(VocabUrl, cancellationToken);
            await File.WriteAllBytesAsync(vocabPath, vocabBytes, cancellationToken);
            logger.LogInformation("Vocab downloaded successfully to {Path}", vocabPath);
        }

        return (modelPath, vocabPath);
    }
}
