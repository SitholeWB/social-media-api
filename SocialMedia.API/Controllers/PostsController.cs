

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public PostsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto, CancellationToken cancellationToken)
    {
        var command = new CreatePostCommand(createPostDto);
        var postId = await _dispatcher.Send<CreatePostCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = postId }, postId);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PostSortBy sortBy = PostSortBy.Latest,
        [FromQuery] Guid? groupId = null,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPostByIdQuery(id);
        var post = await _dispatcher.Query<GetPostByIdQuery, PostDto?>(query, cancellationToken);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportPost(Guid id, [FromBody] ReportPostCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PostId)
        {
            return BadRequest("Post ID mismatch");
        }

        var reportId = await _dispatcher.Send<ReportPostCommand, Guid>(command, cancellationToken);
        return Ok(new { ReportId = reportId });
    }
}
