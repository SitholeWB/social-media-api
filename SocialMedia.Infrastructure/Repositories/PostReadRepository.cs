using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;
using SocialMedia.Infrastructure.Persistence;

namespace SocialMedia.Infrastructure.Repositories;

public class PostReadRepository : IPostReadRepository
{
    private readonly SocialMediaReadDbContext _context;

    public PostReadRepository(SocialMediaReadDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        await _context.Posts.AddAsync(post, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Posts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        return await query
            .OrderByDescending(p => p.Stats.TrendingScore)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetTotalCountAsync(Guid? groupId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
