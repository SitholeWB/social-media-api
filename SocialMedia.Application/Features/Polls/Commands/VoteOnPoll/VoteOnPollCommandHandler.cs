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

        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollOptionId = command.PollOptionId,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote, cancellationToken);
        await _blockchainService.AddVoteAsync(vote.Id, command.UserId, command.PollOptionId, cancellationToken);
        return true;
    }
}
