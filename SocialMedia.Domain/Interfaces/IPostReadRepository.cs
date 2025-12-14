namespace SocialMedia.Domain;

public interface IPostReadRepository
{
    Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default);

    Task<long> GetTotalCountAsync(Guid? groupId = null, CancellationToken cancellationToken = default);
}