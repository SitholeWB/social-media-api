namespace SocialMedia.Application;

public class GetPostsQueryHandler : IQueryHandler<GetPostsQuery, PagedResult<PostDto>>
{
    private readonly IPostReadRepository _readRepository;

    public GetPostsQueryHandler(IPostReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PagedResult<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = request.SortBy == PostSortBy.Trending
            ? await _readRepository.GetTrendingAsync(request.PageNumber, request.PageSize, request.GroupId, cancellationToken)
            : await _readRepository.GetLatestAsync(request.PageNumber, request.PageSize, request.GroupId, cancellationToken);

        var dtos = posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            AuthorId = p.AuthorId,
            AuthorName = p.AuthorName,
            CreatedAt = p.CreatedAt,
            FileUrl = p.FileUrl,
            LikeCount = p.Stats.LikeCount,
            CommentCount = p.Stats.CommentCount,
            AdminTags = p.AdminTags,
            Tags = p.Tags,
            Reactions = p.Reactions
        }).ToList();

        // Get total count for pagination
        var totalCount = await _readRepository.GetTotalCountAsync(request.GroupId, cancellationToken);

        return new PagedResult<PostDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}