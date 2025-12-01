

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public CommentsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto, CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(createCommentDto);
        var commentId = await _dispatcher.Send<CreateCommentCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetCommentById), new { id = commentId }, commentId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCommentByIdQuery(id);
        var comment = await _dispatcher.Query<GetCommentByIdQuery, CommentDto>(query, cancellationToken);
        if (comment == null)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetCommentsByPostId(Guid postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetCommentsByPostIdQuery(postId, pageNumber, pageSize);
        var result = await _dispatcher.Query<GetCommentsByPostIdQuery, PagedResult<CommentDto>>(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] string content, CancellationToken cancellationToken)
    {
        var command = new UpdateCommentCommand(id, content);
        var result = await _dispatcher.Send<UpdateCommentCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCommentCommand(id);
        var result = await _dispatcher.Send<DeleteCommentCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportComment(Guid id, [FromBody] ReportCommentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.CommentId)
        {
            return BadRequest("Comment ID mismatch");
        }

        var reportId = await _dispatcher.Send<ReportCommentCommand, Guid>(command, cancellationToken);
        return Ok(new { ReportId = reportId });
    }

    [HttpGet("/api/v1/posts/{postId}/comments")]
    public async Task<IActionResult> GetPostComments(Guid postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetPostCommentsQuery(postId, pageNumber, pageSize);
        var result = await _dispatcher.Query<GetPostCommentsQuery, PagedResult<CommentReadDto>>(query, cancellationToken);
        return Ok(result);
    }
}
