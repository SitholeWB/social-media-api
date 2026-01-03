namespace SocialMedia.Application;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostDto?>
{
    private readonly IPostReadRepository _postRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetPostByIdQueryHandler(IPostReadRepository postRepository, IUserActivityRepository userActivityRepository)
    {
        _postRepository = postRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
        if (p == null) return null;

        UserActivity? userActivity = null;
        if (request.UserId.HasValue)
        {
            userActivity = await _userActivityRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }

        return new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            AuthorId = p.AuthorId,
            AuthorName = p.AuthorName,
            CreatedAt = p.CreatedAt,
            Media = p.Media,
            LikeCount = p.ReactionCount,
            CommentCount = p.CommentCount,
            UserReaction = userActivity?.Reactions.FirstOrDefault(r => r.EntityId == p.Id && r.EntityType == "Post")?.Emoji,
            AdminTags = p.AdminTags,
            Tags = p.Tags,
            Reactions = p.Reactions,
            StatusFullScreen = p.StatusFullScreen
        };
    }
}