
namespace SocialMedia.Application;

public class GetCommentByIdQuery : IQuery<CommentDto>
{
    public Guid Id { get; }

    public GetCommentByIdQuery(Guid id)
    {
        Id = id;
    }
}

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
