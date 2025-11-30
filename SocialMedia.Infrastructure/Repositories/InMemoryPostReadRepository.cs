using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Infrastructure.Repositories;

public class InMemoryPostReadRepository : IPostReadRepository
{
    // Static list to simulate a persistent store across requests in this demo
    private static readonly List<PostReadModel> _posts = new();

    public Task AddAsync(PostReadModel post)
    {
        _posts.Add(post);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PostReadModel post)
    {
        var existing = _posts.FirstOrDefault(p => p.Id == post.Id);
        if (existing != null)
        {
            _posts.Remove(existing);
            _posts.Add(post);
        }
        return Task.CompletedTask;
    }

    public Task<PostReadModel?> GetByIdAsync(Guid id)
    {
        var post = _posts.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(post);
    }

    public Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null)
    {
        var query = _posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        var items = query
            .OrderByDescending(p => p.TrendingScore)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(items);
    }

    public Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null)
    {
        var query = _posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        var items = query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(items);
    }

    public Task<long> GetTotalCountAsync(Guid? groupId = null)
    {
        var query = _posts.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        return Task.FromResult((long)query.Count());
    }
}
