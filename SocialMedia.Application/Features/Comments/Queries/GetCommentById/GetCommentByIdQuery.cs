namespace SocialMedia.Application;

public record GetCommentByIdQuery(Guid Id) : IQuery<CommentDto>;

public class GetCommentByIdQueryHandler : IQueryHandler<GetCommentByIdQuery, CommentDto>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentByIdQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentDto> Handle(GetCommentByIdQuery query, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(query.Id, cancellationToken);
        if (comment == null)
        {
            // Handle not found, maybe return null or throw exception
            return null!;
        }
        return comment.ToDto();
    }
}