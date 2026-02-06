using Microsoft.Extensions.Caching.Distributed;

namespace SocialMedia.Application;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Guid>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IDistributedCache _cache;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, IDistributedCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task<Guid> HandleAsync(CreateGroupCommand request, CancellationToken cancellationToken)
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

    private async Task InvalidateCacheAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"user_activity_{userId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
    }
}