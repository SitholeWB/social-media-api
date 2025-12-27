namespace SocialMedia.Application;

public record GetMostActivePostsQuery : IQuery<PagedResult<PostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? UserId { get; set; }
}