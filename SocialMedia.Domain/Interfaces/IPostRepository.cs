namespace SocialMedia.Domain;

public interface IPostRepository : IRepository<Post>
{
    Task<(List<Post> Items, long TotalCount)> GetPagedAsync(Guid groupId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}