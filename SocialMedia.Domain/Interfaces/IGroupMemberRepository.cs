namespace SocialMedia.Domain;

public interface IGroupMemberRepository : IRepository<GroupMember>
{
    Task<bool> ExistsAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);

    Task<GroupMember?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
}