using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using SocialMedia.Application;
using SocialMedia.Domain;

namespace SocialMedia.UnitTests.Features.Groups.Queries;

public class GetGroupsQueryHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly GetGroupsQueryHandler _handler;

    public GetGroupsQueryHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _distributedCacheMock = new Mock<IDistributedCache>();
        _handler = new GetGroupsQueryHandler(_groupRepositoryMock.Object, _distributedCacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenGroupsExist()
    {
        // Arrange
        var groups = new List<Group>
        {
            new Group { Id = Guid.NewGuid(), Name = "Group 1" },
            new Group { Id = Guid.NewGuid(), Name = "Group 2" }
        };
        var totalCount = 2;

        _groupRepositoryMock.Setup(x => x.GetGroupsPagedAsync(1, 10, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((groups, totalCount));

        var query = new GetGroupsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.First().Name.Should().Be("Group 1");
        result.Items.First().CreatorId.Should().Be(groups.First().CreatorId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResult_WhenNoGroupsExist()
    {
        // Arrange
        _groupRepositoryMock.Setup(x => x.GetGroupsPagedAsync(1, 10, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Group>(), 0));

        var query = new GetGroupsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}