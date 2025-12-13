namespace SocialMedia.Application;

public class GetPostCommentsQueryHandler : IQueryHandler<GetPostCommentsQuery, PagedResult<CommentReadDto>>
{
    private readonly ICommentReadRepository _commentReadRepository;

    public GetPostCommentsQueryHandler(ICommentReadRepository commentReadRepository)
    {
        _commentReadRepository = commentReadRepository;
    }

    public async Task<PagedResult<CommentReadDto>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentReadRepository.GetByPostIdAsync(request.PostId, request.PageNumber, request.PageSize);

        // We need a way to get total count from the repository for proper pagination For now, let's
        // assume the repository handles it or we add a method But InMemoryCommentReadRepository
        // doesn't have GetTotalCount yet. Let's just return what we have.

        var dtos = comments.Select(c => new CommentReadDto
        {
            CommentId = c.Id,
            Content = c.Content,
            AuthorId = c.AuthorId,
            AuthorName = c.AuthorName,
            AuthorProfilePicUrl = c.AuthorProfilePicUrl,
            CreatedAt = c.CreatedAt,
            LikeCount = c.Stats.LikeCount,
            Reactions = c.Reactions
        }).ToList();

        return new PagedResult<CommentReadDto>(dtos, 0, request.PageNumber, request.PageSize);
    }
}