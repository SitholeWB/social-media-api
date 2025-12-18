using FluentAssertions;
using Moq;
using SocialMedia.Application;
using SocialMedia.Domain;

namespace SocialMedia.UnitTests.Features.Polls.Commands;

public class UpdatePollCommandHandlerTests
{
    private readonly Mock<IPollRepository> _pollRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IGroupMemberRepository> _groupMemberRepositoryMock;
    private readonly UpdatePollCommandHandler _handler;

    public UpdatePollCommandHandlerTests()
    {
        _pollRepositoryMock = new Mock<IPollRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _groupMemberRepositoryMock = new Mock<IGroupMemberRepository>();
        _handler = new UpdatePollCommandHandler(_pollRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePoll_WhenPollExists()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        var poll = new Poll { Id = pollId, Question = "Old Question", IsActive = true };

        _pollRepositoryMock.Setup(x => x.GetByIdAsync(pollId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(poll);

        var command = new UpdatePollCommand(pollId, "New Question", false, DateTime.UtcNow.AddDays(1), true, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        poll.Question.Should().Be("New Question");
        poll.IsActive.Should().BeFalse();
        _pollRepositoryMock.Verify(x => x.UpdateAsync(poll, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenPollDoesNotExist()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        _pollRepositoryMock.Setup(x => x.GetByIdAsync(pollId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Poll?)null);

        var command = new UpdatePollCommand(pollId, "New Question", false, null, false, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _pollRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Poll>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
