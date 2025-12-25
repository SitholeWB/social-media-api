using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SocialMedia.Infrastructure;
using Xunit;

namespace SocialMedia.UnitTests.Infrastructure;

public class OnnxEmbeddingGeneratorTests
{
    private readonly Mock<ILogger<OnnxEmbeddingGenerator>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public OnnxEmbeddingGeneratorTests()
    {
        _loggerMock = new Mock<ILogger<OnnxEmbeddingGenerator>>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup default configuration if needed
        _configurationMock.Setup(c => c["EmbeddingSettings:ModelPath"]).Returns("Models/bge-micro-v2/model.onnx");
        _configurationMock.Setup(c => c["EmbeddingSettings:VocabPath"]).Returns("Models/bge-micro-v2/vocab.txt");
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_ShouldReturnVectorOfCorrectDimension()
    {
        // Arrange
        var generator = new OnnxEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        var text = "This is a test sentence for embedding generation.";

        // Act
        var embedding = await generator.GenerateEmbeddingAsync(text);

        // Assert
        Assert.Equal(384, embedding.Length);
        Assert.Equal(384, generator.Dimensions);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_SameText_ShouldProduceSameEmbedding()
    {
        // Arrange
        var generator = new OnnxEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        var text = "Consistency check.";

        // Act
        var embedding1 = await generator.GenerateEmbeddingAsync(text);
        var embedding2 = await generator.GenerateEmbeddingAsync(text);

        // Assert
        Assert.Equal(embedding1.ToArray(), embedding2.ToArray());
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_DifferentText_ShouldProduceDifferentEmbedding()
    {
        // Arrange
        var generator = new OnnxEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        var text1 = "The quick brown fox jumps over the lazy dog.";
        var text2 = "Artificial intelligence is changing the world.";

        // Act
        var embedding1 = await generator.GenerateEmbeddingAsync(text1);
        var embedding2 = await generator.GenerateEmbeddingAsync(text2);

        // Assert
        Assert.NotEqual(embedding1.ToArray(), embedding2.ToArray());
    }
}
