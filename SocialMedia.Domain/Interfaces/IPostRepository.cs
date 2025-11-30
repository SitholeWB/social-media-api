namespace SocialMedia.Domain;

public interface IPostRepository : IRepository<Post>
{
    Task<(List<Post> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
