namespace SocialMedia.Application;

public enum PostSortBy
{
    Latest,
    Trending
}

public record GetPostsQuery(Guid GroupId) : IQuery<PagedResult<PostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public PostSortBy SortBy { get; set; } = PostSortBy.Latest;
}