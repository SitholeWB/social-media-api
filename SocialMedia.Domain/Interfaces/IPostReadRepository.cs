namespace SocialMedia.Domain;

public interface IPostReadRepository
{
    Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default);

    Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<long> GetTotalCountAsync(Guid groupId, CancellationToken cancellationToken);
}