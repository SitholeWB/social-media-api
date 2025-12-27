namespace SocialMedia.Domain;

public interface IPostReadRepository
{
    Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetTrendingAsync(Guid groupId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetLatestAsync(Guid groupId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<long> GetTotalCountAsync(Guid groupId, CancellationToken cancellationToken = default);

    // Global queries (across all groups)
    Task<List<PostReadModel>> GetGlobalTrendingAsync(int page, int pageSize, int daysBack, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetMostActiveAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<List<PostReadModel>> GetMostAttractiveAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<long> GetGlobalTotalCountAsync(CancellationToken cancellationToken = default);
}