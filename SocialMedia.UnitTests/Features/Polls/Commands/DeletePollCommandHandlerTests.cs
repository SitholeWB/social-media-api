using FluentAssertions;
using Moq;
using SocialMedia.Application;
using SocialMedia.Domain;

namespace SocialMedia.UnitTests.Features.Polls.Commands;

public class DeletePollCommandHandlerTests
{
    private readonly Mock<IPollRepository> _pollRepositoryMock;
    private readonly DeletePollCommandHandler _handler;

    public DeletePollCommandHandlerTests()
    {
        _pollRepositoryMock = new Mock<IPollRepository>();
        _handler = new DeletePollCommandHandler(_pollRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeletePoll_WhenPollExists()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        var poll = new Poll { Id = pollId, Question = "Test Poll" };

        _pollRepositoryMock.Setup(x => x.GetByIdAsync(pollId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(poll);

        var command = new DeletePollCommand(pollId);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _pollRepositoryMock.Verify(x => x.DeleteAsync(poll, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenPollDoesNotExist()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        _pollRepositoryMock.Setup(x => x.GetByIdAsync(pollId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Poll?)null);

        var command = new DeletePollCommand(pollId);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _pollRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Poll>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}