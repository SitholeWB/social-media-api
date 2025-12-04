using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class DeletePollCommandHandler : ICommandHandler<DeletePollCommand, bool>
{
    private readonly SocialMediaDbContext _context;

    public DeletePollCommandHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == command.PollId, cancellationToken);
        if (poll == null)
        {
            return false;
        }

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
