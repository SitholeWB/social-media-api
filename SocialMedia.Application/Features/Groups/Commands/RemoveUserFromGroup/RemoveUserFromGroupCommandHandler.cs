namespace SocialMedia.Application;

public class RemoveUserFromGroupCommandHandler : ICommandHandler<RemoveUserFromGroupCommand, bool>
{
    private readonly IGroupMemberRepository _groupMemberRepository;

    public RemoveUserFromGroupCommandHandler(IGroupMemberRepository groupMemberRepository)
    {
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task<bool> Handle(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        var member = await _groupMemberRepository.GetByGroupAndUserAsync(request.GroupId, request.UserId, cancellationToken);

        if (member == null)
        {
            return false;
        }

        await _groupMemberRepository.DeleteAsync(member, cancellationToken);

        return true;
    }
}
