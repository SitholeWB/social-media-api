

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public GroupsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var groupId = await _dispatcher.Send<CreateGroupCommand, Guid>(command, cancellationToken);
        return Ok(groupId);
    }

    [HttpPost("{groupId}/users/{userId}")]
    public async Task<IActionResult> AddUserToGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new AddUserToGroupCommand(groupId, userId);
        await _dispatcher.Send<AddUserToGroupCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("{groupId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromGroupCommand(groupId, userId);
        await _dispatcher.Send<RemoveUserFromGroupCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpGet("{groupId}/posts")]
    public async Task<IActionResult> GetGroupPosts(
        Guid groupId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PostSortBy sortBy = PostSortBy.Latest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPostsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            GroupId = groupId
        };
        var result = await _dispatcher.Query<GetPostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }
}
