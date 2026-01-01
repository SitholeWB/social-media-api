namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/groups")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return BadRequest("Failed to get user from auth token");
        }
        var commandWithCreator = command with { CreatorId = userId.Value };
        var groupId = await _dispatcher.Send<CreateGroupCommand, Guid>(commandWithCreator, cancellationToken);
        return Ok(groupId);
    }

    [HttpPost("{groupId}/users/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserToGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new AddUserToGroupCommand(groupId, userId);
        await _dispatcher.Send<AddUserToGroupCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("{groupId}/users/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveUserFromGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromGroupCommand(groupId, userId);
        await _dispatcher.Send<RemoveUserFromGroupCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpPost("{groupId}/posts")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto, [FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        createPostDto.GroupId = groupId;
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return BadRequest("Failed to get user from auth token");
        }
        createPostDto.AuthorId = userId.Value;
        var command = new CreatePostCommand(createPostDto);
        var postId = await _dispatcher.Send<CreatePostCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(CreatePost), new { id = postId }, postId);
    }

    [AllowAnonymous]
    [HttpGet("{groupId}/posts")]
    public async Task<IActionResult> GetGroupPosts(
        Guid groupId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PostSortBy sortBy = PostSortBy.Latest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPostsQuery(groupId)
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            UserId = this.GetUserId()
        };
        var result = await _dispatcher.Query<GetPostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetGroups([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, bool includeDefaults = false, CancellationToken cancellationToken = default)
    {
        var query = new GetGroupsQuery(pageNumber, pageSize, includeDefaults);
        var result = await _dispatcher.Query<GetGroupsQuery, PagedResult<GroupDto>>(query, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetGroupQuery(id);
        var result = await _dispatcher.Query<GetGroupQuery, GroupDto?>(query, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        if (id != command.GroupId)
        {
            return BadRequest();
        }

        var result = await _dispatcher.Send<UpdateGroupCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteGroupCommand(id);
        var result = await _dispatcher.Send<DeleteGroupCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("{groupId}/polls")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePoll(Guid groupId, [FromBody] CreatePollCommand command, CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return BadRequest("Failed to get user from auth token");
        }
        var commandWithGroup = command with { GroupId = groupId, CreatorId = userId.Value };
        var pollId = await _dispatcher.Send<CreatePollCommand, Guid>(commandWithGroup, cancellationToken);
        return Ok(pollId);
    }

    [AllowAnonymous]
    [HttpGet("{groupId}/polls")]
    public async Task<IActionResult> GetGroupPolls(Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetActivePollsQuery(groupId, pageNumber, pageSize)
        {
            UserId = this.GetUserId()
        };
        var result = await _dispatcher.Query<GetActivePollsQuery, PagedResult<PollDto>>(query, cancellationToken);
        return Ok(result);
    }
}