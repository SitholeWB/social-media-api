namespace SocialMedia.Application;

public record GetCommentByIdQuery(Guid Id) : IQuery<CommentReadDto>
{
    public Guid? UserId { get; set; }
}

public class GetCommentByIdQueryHandler : IQueryHandler<GetCommentByIdQuery, CommentReadDto>
{
    private readonly ICommentReadRepository _commentRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetCommentByIdQueryHandler(ICommentReadRepository commentRepository, IUserActivityRepository userActivityRepository)
    {
        _commentRepository = commentRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<CommentReadDto> Handle(GetCommentByIdQuery query, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(query.Id, cancellationToken);
        if (comment == null)
        {
            return null!;
        }

        UserActivity? userActivity = null;
        if (query.UserId.HasValue)
        {
            userActivity = await _userActivityRepository.GetByUserIdAsync(query.UserId.Value, cancellationToken);
        }

        return new CommentReadDto
        {
            CommentId = comment.Id,
            Content = comment.Content,
            AuthorId = comment.AuthorId,
            AuthorName = comment.AuthorName,
            AuthorProfilePicUrl = comment.AuthorProfilePicUrl,
            CreatedAt = comment.CreatedAt,
            LikeCount = comment.Stats.LikeCount,
            UserReaction = userActivity?.Reactions.FirstOrDefault(r => r.EntityId == comment.Id && r.EntityType == "Comment")?.Emoji,
            Reactions = comment.Reactions,
            Tags = comment.Tags,
            AdminTags = comment.AdminTags
        };
    }
}