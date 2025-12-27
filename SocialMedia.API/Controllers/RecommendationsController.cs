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

        var result = await _dispatcher.Query<GetRecommendedPostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get trending posts from the last N days
    /// </summary>
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int daysBack = 7,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTrendingPostsQuery
        {
            UserId = this.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            DaysBack = daysBack
        };

        var result = await _dispatcher.Query<GetTrendingPostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get most active posts (by comment count)
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetMostActive(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMostActivePostsQuery
        {
            UserId = this.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _dispatcher.Query<GetMostActivePostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get most attractive posts (by like count)
    /// </summary>
    [HttpGet("attractive")]
    public async Task<IActionResult> GetMostAttractive(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMostAttractivePostsQuery
        {
            UserId = this.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _dispatcher.Query<GetMostAttractivePostsQuery, PagedResult<PostDto>>(query, cancellationToken);
        return Ok(result);
    }
}