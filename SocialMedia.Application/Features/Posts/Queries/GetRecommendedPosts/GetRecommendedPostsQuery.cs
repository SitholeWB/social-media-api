namespace SocialMedia.Application;

public record GetRecommendedPostsQuery : IQuery<PagedResult<PostDto>>
{
    public Guid? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}