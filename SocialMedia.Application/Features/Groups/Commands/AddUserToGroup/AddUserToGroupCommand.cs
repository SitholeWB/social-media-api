namespace SocialMedia.Application;

public record AddUserToGroupCommand(Guid GroupId, Guid UserId) : ICommand<bool>;

public class AddUserToGroupCommandHandler : ICommandHandler<AddUserToGroupCommand, bool>
{
    private readonly IGroupMemberRepository _groupMemberRepository;

    public AddUserToGroupCommandHandler(IGroupMemberRepository groupMemberRepository)
    {
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task<bool> HandleAsync(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var exists = await _groupMemberRepository.ExistsAsync(request.GroupId, request.UserId, cancellationToken);

        if (exists)
        {
            return true;
        }

        var member = new GroupMember
        {
            GroupId = request.GroupId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _groupMemberRepository.AddAsync(member, cancellationToken);

        return true;
    }
}