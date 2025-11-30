using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Domain.Interfaces;

public interface IPostReadRepository
{
    Task AddAsync(PostReadModel post);
    Task UpdateAsync(PostReadModel post);
    Task<PostReadModel?> GetByIdAsync(Guid id);
    Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null);
    Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null);
    Task<long> GetTotalCountAsync(Guid? groupId = null);
}
