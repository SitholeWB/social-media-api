namespace SocialMedia.Application;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Guid>
{
    private readonly IGroupRepository _groupRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            CreatorId = request.CreatorId,
            CreatedAt = DateTime.UtcNow
        };

        var createdGroup = await _groupRepository.AddAsync(group, cancellationToken);

        return createdGroup.Id;
    }
}
