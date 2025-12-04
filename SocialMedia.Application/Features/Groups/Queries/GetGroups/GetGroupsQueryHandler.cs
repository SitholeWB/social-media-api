using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, PagedResult<GroupDto>>
{
    private readonly SocialMediaDbContext _context;

    public GetGroupsQueryHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<GroupDto>> Handle(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        var queryable = _context.Groups.AsNoTracking();

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .OrderByDescending(g => g.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsPublic = g.IsPublic,
                IsAutoAdd = g.IsAutoAdd,
                CreatorId = g.CreatorId,
                CreatedAt = g.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<GroupDto>(items, totalCount, query.PageNumber, query.PageSize);
    }
}
