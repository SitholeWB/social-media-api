namespace SocialMedia.Application;

public class BlockUserCommandHandler : ICommandHandler<BlockUserCommand, bool>
{
    private readonly IUserBlockRepository _userBlockRepository;

    public BlockUserCommandHandler(IUserBlockRepository userBlockRepository)
    {
        _userBlockRepository = userBlockRepository;
    }

    public async Task<bool> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        if (request.BlockerId == request.BlockedUserId)
        {
            throw new ArgumentException("Cannot block yourself");
        }

        var exists = await _userBlockRepository.ExistsAsync(request.BlockerId, request.BlockedUserId, cancellationToken);

        if (exists)
        {
            return true; // Already blocked
        }

        var block = new UserBlock
        {
            BlockerId = request.BlockerId,
            BlockedUserId = request.BlockedUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _userBlockRepository.AddAsync(block, cancellationToken);

        return true;
    }
}