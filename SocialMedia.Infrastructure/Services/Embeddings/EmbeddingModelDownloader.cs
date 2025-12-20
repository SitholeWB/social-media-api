namespace SocialMedia.Infrastructure;

/// <summary>
/// Utility to download ONNX embedding model files
/// </summary>
public static class EmbeddingModelDownloader
{
    private const string ModelUrl = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/onnx/model.onnx";
    private const string TokenizerUrl = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/resolve/main/tokenizer.json";

    public static async Task<(string modelPath, string tokenizerPath)> EnsureModelsDownloadedAsync(
        string modelsDirectory,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(modelsDirectory);

        var modelPath = Path.Combine(modelsDirectory, "all-MiniLM-L6-v2.onnx");
        var tokenizerPath = Path.Combine(modelsDirectory, "tokenizer.json");

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

        // Download tokenizer if not exists
        if (!File.Exists(tokenizerPath))
        {
            logger.LogInformation("Downloading tokenizer from {Url}...", TokenizerUrl);
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            var tokenizerBytes = await httpClient.GetByteArrayAsync(TokenizerUrl, cancellationToken);
            await File.WriteAllBytesAsync(tokenizerPath, tokenizerBytes, cancellationToken);
            logger.LogInformation("Tokenizer downloaded successfully to {Path}", tokenizerPath);
        }

        return (modelPath, tokenizerPath);
    }
}
