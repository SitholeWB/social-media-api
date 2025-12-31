namespace SocialMedia.Application;

public class GetPostCommentsQueryHandler : IQueryHandler<GetPostCommentsQuery, PagedResult<CommentReadDto>>
{
    private readonly ICommentReadRepository _commentReadRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetPostCommentsQueryHandler(ICommentReadRepository commentReadRepository, IUserActivityRepository userActivityRepository)
    {
        _commentReadRepository = commentReadRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<PagedResult<CommentReadDto>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentReadRepository.GetByPostIdAsync(request.PostId, request.PageNumber, request.PageSize);

        UserActivity? userActivity = null;
        if (request.UserId.HasValue)
        {
            userActivity = await _userActivityRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }

        var dtos = comments.Select(c => new CommentReadDto
        {
            CommentId = c.Id,
            Content = c.Content,
            AuthorId = c.AuthorId,
            AuthorName = c.AuthorName,
            AuthorProfilePicUrl = c.AuthorProfilePicUrl,
            Media = c.Media,
            CreatedAt = c.CreatedAt,
            LikeCount = c.Stats.LikeCount,
            UserReaction = userActivity?.Reactions.FirstOrDefault(r => r.EntityId == c.Id && r.EntityType == "Comment")?.Emoji,
            Reactions = c.Reactions,
            Tags = c.Tags,
            AdminTags = c.AdminTags
        }).ToList();

        return new PagedResult<CommentReadDto>(dtos, 0, request.PageNumber, request.PageSize);
    }
}