namespace SocialMedia.Domain;

public interface ICommentRepository : IRepository<Comment>
{
    Task<(List<Comment> Items, long TotalCount)> GetPagedByPostIdAsync(Guid postId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
