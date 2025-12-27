namespace SocialMedia.Domain;

public interface IBlockchainService
{
    Task AddVoteAsync(Guid voteId, Guid userId, Guid pollOptionId, CancellationToken cancellationToken = default);

    Task<bool> VerifyChainAsync(CancellationToken cancellationToken = default);
}