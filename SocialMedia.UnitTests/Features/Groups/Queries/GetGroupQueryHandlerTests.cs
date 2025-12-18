using FluentAssertions;
using Moq;
using SocialMedia.Application;
using SocialMedia.Domain;

namespace SocialMedia.UnitTests.Features.Groups.Queries;

public class GetGroupQueryHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly GetGroupQueryHandler _handler;

    public GetGroupQueryHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _handler = new GetGroupQueryHandler(_groupRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnGroupDto_WhenGroupExists()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var group = new Group
        {
            Id = groupId,
            Name = "Test Group",
            Description = "Test Description",
            IsPublic = true,
            IsAutoAdd = false,
            Type = GroupType.Public,
            CreatedAt = DateTime.UtcNow
        };

        _groupRepositoryMock.Setup(x => x.GetByIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var query = new GetGroupQuery(groupId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(groupId);
        result.Name.Should().Be(group.Name);
        result.Description.Should().Be(group.Description);
        result.IsPublic.Should().Be(group.IsPublic);
        result.IsAutoAdd.Should().Be(group.IsAutoAdd);
        result.Type.Should().Be(group.Type);
        result.CreatorId.Should().Be(group.CreatorId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        _groupRepositoryMock.Setup(x => x.GetByIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var query = new GetGroupQuery(groupId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
