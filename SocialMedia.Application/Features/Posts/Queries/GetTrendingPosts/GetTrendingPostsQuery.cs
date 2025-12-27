namespace SocialMedia.Application;

public record GetTrendingPostsQuery : IQuery<PagedResult<PostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? UserId { get; set; }
    public int DaysBack { get; set; } = 7; // Look at posts from last N days
}