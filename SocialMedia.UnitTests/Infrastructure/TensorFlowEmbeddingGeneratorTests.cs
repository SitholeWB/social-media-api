using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SocialMedia.Infrastructure;
using Xunit;

namespace SocialMedia.UnitTests.Infrastructure;

public class TensorFlowEmbeddingGeneratorTests
{
    private readonly Mock<ILogger<TensorFlowEmbeddingGenerator>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public TensorFlowEmbeddingGeneratorTests()
    {
        _loggerMock = new Mock<ILogger<TensorFlowEmbeddingGenerator>>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Use default paths (null) to trigger internal logic or set explicit correct path
        // _configurationMock.Setup(c => c["EmbeddingSettings:ModelPath"]).Returns((string)null);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_ShouldReturnVectorOfCorrectDimension()
    {
        // Arrange
        var generator = new TensorFlowEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        var text = "This is a test sentence for embedding generation.";

        // Act
        var embedding = await generator.GenerateEmbeddingAsync(text);

        // Assert
        Assert.Equal(128, embedding.Length);
        Assert.Equal(128, generator.Dimensions);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_SameText_ShouldProduceSameEmbedding()
    {
        // Arrange
        var generator = new TensorFlowEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        var text = "Consistency check.";

        // Act
        var embedding1 = await generator.GenerateEmbeddingAsync(text);
        var embedding2 = await generator.GenerateEmbeddingAsync(text);

        // Assert
        Assert.Equal(embedding1.ToArray(), embedding2.ToArray());
    }

    [Fact]
    public void Dimensions_ShouldBe128()
    {
        // Simple test that doesn't require the model to be loaded
        var generator = new TensorFlowEmbeddingGenerator(_configurationMock.Object, _loggerMock.Object);
        Assert.Equal(128, generator.Dimensions);
    }
}
