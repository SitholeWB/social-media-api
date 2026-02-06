namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/recommendations")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public RecommendationsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Get recommended posts based on vector similarity
    /// </summary>
    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommended(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecommendedPostsQuery
        {
            UserId = this.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _dispatcher.QueryAsync<GetRecommendedPostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }
}