

namespace SocialMedia.Application;

public class GetCommentsByPostIdQuery : IQuery<PagedResult<CommentDto>>
{
    public Guid PostId { get; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetCommentsByPostIdQuery(Guid postId, int pageNumber, int pageSize)
    {
        PostId = postId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public class GetCommentsByPostIdQueryHandler : IQueryHandler<GetCommentsByPostIdQuery, PagedResult<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsByPostIdQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<PagedResult<CommentDto>> Handle(GetCommentsByPostIdQuery query, CancellationToken cancellationToken)
    {
        var (comments, totalCount) = await _commentRepository.GetPagedByPostIdAsync(query.PostId, query.PageNumber, query.PageSize, cancellationToken);
        var dtos = comments.Select(c => c.ToDto()).ToList();
        return new PagedResult<CommentDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}
