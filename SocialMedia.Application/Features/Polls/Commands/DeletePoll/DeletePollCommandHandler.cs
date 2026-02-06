namespace SocialMedia.Application;

public class DeletePollCommandHandler : ICommandHandler<DeletePollCommand, bool>
{
    private readonly IPollRepository _pollRepository;

    public DeletePollCommandHandler(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<bool> HandleAsync(DeletePollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(command.PollId, cancellationToken);
        if (poll == null)
        {
            return false;
        }

        await _pollRepository.DeleteAsync(poll, cancellationToken);
        return true;
    }
}