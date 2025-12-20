namespace SocialMedia.Application;

public class GetRecommendedPostsQueryHandler : IQueryHandler<GetRecommendedPostsQuery, PagedResult<PostDto>>
{
    private readonly IPostVectorService _vectorService;
    private readonly IPostReadRepository _readRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetRecommendedPostsQueryHandler(
        IPostVectorService vectorService,
        IPostReadRepository readRepository,
        IUserActivityRepository userActivityRepository)
    {
        _vectorService = vectorService;
        _readRepository = readRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<PagedResult<PostDto>> Handle(GetRecommendedPostsQuery request, CancellationToken cancellationToken)
    {
        // Get recommended post IDs from vector service
        var recommendedPostIds = await _vectorService.GetRecommendedPostIdsAsync(
            request.UserId,
            request.PageSize * request.PageNumber, // Get enough for pagination
            cancellationToken);

        // Apply pagination to the IDs
        var pagedPostIds = recommendedPostIds
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        if (!pagedPostIds.Any())
        {
            return new PagedResult<PostDto>(new List<PostDto>(), 0, request.PageNumber, request.PageSize);
        }

        // Fetch full post details
        var posts = new List<PostReadModel>();
        foreach (var postId in pagedPostIds)
        {
            var post = await _readRepository.GetByIdAsync(postId, cancellationToken);
            if (post != null)
            {
                posts.Add(post);
            }
        }

        // Get user activity for reactions
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

        return new PagedResult<PostDto>(dtos, recommendedPostIds.Count, request.PageNumber, request.PageSize);
    }
}
