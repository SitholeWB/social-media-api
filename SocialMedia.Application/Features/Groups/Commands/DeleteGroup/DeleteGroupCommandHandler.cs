using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class DeleteGroupCommandHandler : ICommandHandler<DeleteGroupCommand, bool>
{
    private readonly SocialMediaDbContext _context;

    public DeleteGroupCommandHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == command.GroupId, cancellationToken);
        if (group == null)
        {
            return false;
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
