namespace SocialMedia.Domain;

public interface ICommentReadRepository
{
    Task AddAsync(CommentReadModel comment, CancellationToken cancellationToken = default);

    Task UpdateAsync(CommentReadModel comment, CancellationToken cancellationToken = default);

    Task<CommentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize, CancellationToken cancellationToken = default);
}