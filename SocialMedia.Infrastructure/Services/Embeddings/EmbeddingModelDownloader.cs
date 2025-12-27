namespace SocialMedia.Infrastructure;

/// <summary>
/// Utility to download ONNX embedding model files
/// </summary>
public static class EmbeddingModelDownloader
{
    private const string ModelUrl = "https://tfhub.dev/tensorflow/small_bert/bert_en_uncased_L-2_H-128_A-2/1?tf-hub-format=compressed";
    private const string VocabUrl = "https://huggingface.co/google/bert_uncased_L-2_H-128_A-2/resolve/main/vocab.txt";

    public static async Task<(string modelPath, string vocabPath)> EnsureModelsDownloadedAsync(
        string modelsDirectory,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(modelsDirectory);

        var savedModelDir = Path.Combine(modelsDirectory, "saved_model");
        var modelPath = Path.Combine(savedModelDir, "saved_model.pb");
        var vocabPath = Path.Combine(modelsDirectory, "vocab.txt");

        // Download model if not exists
        if (!File.Exists(modelPath))
        {
            logger.LogInformation("Downloading TinyBERT SavedModel from {Url}...", ModelUrl);
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            
            var response = await httpClient.GetAsync(ModelUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Note: Since this is a .tar.gz from TF Hub, in a real production scenario 
            // you'd use a library to extract it. For now, I'll provide instructions 
            // or assume a simpler direct link if found. 
            // For this implementation, I'll update the generator to load from the directory.
            var tempFile = Path.Combine(modelsDirectory, "model.tar.gz");
            using (var fs = new FileStream(tempFile, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs, cancellationToken);
            }
            
            logger.LogInformation("Extracting model to {Dir}...", savedModelDir);
            if (Directory.Exists(savedModelDir)) Directory.Delete(savedModelDir, true);
            Directory.CreateDirectory(savedModelDir);

            using (var stream = File.OpenRead(tempFile))
            using (var gzipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress))
            {
                System.Formats.Tar.TarFile.ExtractToDirectory(gzipStream, savedModelDir, true);
            }
            
            File.Delete(tempFile);

            // Verify extraction
            if (!File.Exists(modelPath))
            {
                // Sometimes the tar contains a subfolder. Check for it.
                var subDirs = Directory.GetDirectories(savedModelDir);
                if (subDirs.Length > 0 && File.Exists(Path.Combine(subDirs[0], "saved_model.pb")))
                {
                    logger.LogInformation("Found model in subfolder. Moving files...");
                    foreach (var file in Directory.GetFiles(subDirs[0]))
                        File.Move(file, Path.Combine(savedModelDir, Path.GetFileName(file)), true);
                    foreach (var dir in Directory.GetDirectories(subDirs[0]))
                        Directory.Move(dir, Path.Combine(savedModelDir, Path.GetFileName(dir)));
                    Directory.Delete(subDirs[0], true);
                }
            }

            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"Model file {modelPath} not found after extraction.");

            logger.LogInformation("Model extracted and verified successfully at {Path}", savedModelDir);
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
