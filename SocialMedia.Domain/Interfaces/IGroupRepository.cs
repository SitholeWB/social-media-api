namespace SocialMedia.Domain;

public interface IGroupRepository : IRepository<Group>
{
    Task<(List<Group> Items, long TotalCount)> GetGroupsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
