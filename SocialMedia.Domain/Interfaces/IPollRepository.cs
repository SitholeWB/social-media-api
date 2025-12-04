namespace SocialMedia.Domain;

public interface IPollRepository
{
    Task<Poll?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Poll poll, CancellationToken cancellationToken = default);
    Task UpdateAsync(Poll poll, CancellationToken cancellationToken = default);
    Task<bool> HasUserVotedAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<Poll>> GetActivePollsAsync(CancellationToken cancellationToken = default);
    Task<(List<Poll> Items, long TotalCount)> GetActivePollsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task DeleteAsync(Poll poll, CancellationToken cancellationToken = default);
}
