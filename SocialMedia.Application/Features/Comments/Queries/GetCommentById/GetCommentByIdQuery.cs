namespace SocialMedia.Application;

public record GetCommentByIdQuery(Guid Id) : IQuery<CommentReadDto>;

public class GetCommentByIdQueryHandler : IQueryHandler<GetCommentByIdQuery, CommentReadDto>
{
    private readonly ICommentReadRepository _commentRepository;

    public GetCommentByIdQueryHandler(ICommentReadRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentReadDto> Handle(GetCommentByIdQuery query, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(query.Id, cancellationToken);
        if (comment == null)
        {
            // Handle not found, maybe return null or throw exception
            return null!;
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
            Reactions = comment.Reactions,
            Tags = comment.Tags,
            AdminTags = comment.AdminTags
        };
    }
}