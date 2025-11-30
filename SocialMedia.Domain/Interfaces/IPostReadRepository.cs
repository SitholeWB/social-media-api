using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Domain.Interfaces;

public interface IPostReadRepository
{
    Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default);
    Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default);
    Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default);
    Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default);
    Task<long> GetTotalCountAsync(Guid? groupId = null, CancellationToken cancellationToken = default);
}
