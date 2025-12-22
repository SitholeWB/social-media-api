namespace SocialMedia.Domain;

public interface IGroupRepository : IRepository<Group>
{
    Task<(List<Group> Items, long TotalCount)> GetGroupsPagedAsync(int pageNumber, int pageSize, bool includeDefaults = false, CancellationToken cancellationToken = default);
}