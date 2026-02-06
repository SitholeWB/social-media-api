using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SocialMedia.Application;

public class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, PagedResult<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IDistributedCache _cache;

    public GetGroupsQueryHandler(IGroupRepository groupRepository, IDistributedCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task<PagedResult<GroupDto>> HandleAsync(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        //var cacheKey = $"groups_page_{query.PageNumber}_size_{query.PageSize}";

        //// Try get from cache
        //var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        //if (!string.IsNullOrEmpty(cachedData))
        //{
        //    return JsonSerializer.Deserialize<PagedResult<GroupDto>>(cachedData)!;
        //}

        var (items, totalCount) = await _groupRepository.GetGroupsPagedAsync(query.PageNumber, query.PageSize, query.IncludeDefaults, cancellationToken);

        var dtos = items.Select(g => new GroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Type = g.Type,
            CreatorId = g.CreatorId,
            CreatedAt = g.CreatedAt
        }).ToList();

        var result = new PagedResult<GroupDto>(dtos, totalCount, query.PageNumber, query.PageSize);

        // Set to cache
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache groups for 10 minutes
        };
        var serialized = JsonSerializer.Serialize(result);
        //await _cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);

        return result;
    }
}