namespace SocialMedia.Application;

public class VoteOnPollCommandHandler : ICommandHandler<VoteOnPollCommand, bool>
{
    private readonly IPollRepository _pollRepository;
    private readonly IRepository<Vote> _voteRepository;
    private readonly IBlockchainService _blockchainService;

    public VoteOnPollCommandHandler(IPollRepository pollRepository, IRepository<Vote> voteRepository, IBlockchainService blockchainService)
    {
        _pollRepository = pollRepository;
        _voteRepository = voteRepository;
        _blockchainService = blockchainService;
    }

    public async Task<bool> Handle(VoteOnPollCommand command, System.Threading.CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(command.PollId, cancellationToken);
        if (poll == null || !poll.IsActive || (poll.ExpiresAt.HasValue && poll.ExpiresAt < DateTime.UtcNow))
        {
            return false;
        }

        var hasVoted = await _pollRepository.HasUserVotedAsync(command.PollId, command.UserId, cancellationToken);
        if (hasVoted)
        {
            throw new InvalidOperationException("User has already voted on this poll.");
        }

        // 1. Record that the user has voted (Anonymized duplicate prevention)
        var voteRecord = new PollVoteRecord
        {
            Id = Guid.NewGuid(),
            PollId = command.PollId,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };
        await _pollRepository.AddVoteRecordAsync(voteRecord, cancellationToken);

        // 2. Record the actual vote selection
        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollOptionId = command.PollOptionId,
            UserId = poll.IsAnonymous ? null : command.UserId, // SEPARATION: selection is anonymous if requested
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote, cancellationToken);
        
        // 3. Update blockchain (Immutable audit log)
        // For anonymous polls, we record Guid.Empty as UserId to maintain total anonymity on the ledger
        var userIdForBlockchain = poll.IsAnonymous ? Guid.Empty : command.UserId;
        await _blockchainService.AddVoteAsync(vote.Id, userIdForBlockchain, command.PollOptionId, cancellationToken);
        
        return true;
    }
}
