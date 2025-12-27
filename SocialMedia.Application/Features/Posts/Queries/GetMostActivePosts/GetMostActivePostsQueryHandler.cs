namespace SocialMedia.Application;

public class GetMostActivePostsQueryHandler : IQueryHandler<GetMostActivePostsQuery, PagedResult<PostDto>>
{
    private readonly IPostReadRepository _readRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetMostActivePostsQueryHandler(
        IPostReadRepository readRepository,
        IUserActivityRepository userActivityRepository)
    {
        _readRepository = readRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<PagedResult<PostDto>> Handle(GetMostActivePostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _readRepository.GetMostActiveAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        UserActivity? userActivity = null;
        if (request.UserId.HasValue)
        {
            userActivity = await _userActivityRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }

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
            UserReaction = userActivity?.Reactions.FirstOrDefault(r => r.EntityId == p.Id && r.EntityType == "Post")?.Emoji,
            AdminTags = p.AdminTags,
            Tags = p.Tags,
            Reactions = p.Reactions,
            StatusFullScreen = p.StatusFullScreen,
        }).ToList();

        var totalCount = await _readRepository.GetGlobalTotalCountAsync(cancellationToken);

        return new PagedResult<PostDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}