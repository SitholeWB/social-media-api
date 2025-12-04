using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class GetGroupQueryHandler : IQueryHandler<GetGroupQuery, GroupDto?>
{
    private readonly SocialMediaDbContext _context;

    public GetGroupQueryHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<GroupDto?> Handle(GetGroupQuery query, CancellationToken cancellationToken)
    {
        return await _context.Groups
            .AsNoTracking()
            .Where(g => g.Id == query.GroupId)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
