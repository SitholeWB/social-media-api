using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Infrastructure.Repositories;

public class InMemoryPostReadRepository : IPostReadRepository
{
    // Using ConcurrentDictionary for thread safety
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, PostReadModel> _posts = new();

    public Task AddAsync(PostReadModel post)
    {
        _posts.TryAdd(post.Id, post);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PostReadModel post)
    {
        _posts.AddOrUpdate(post.Id, post, (key, oldValue) => post);
        return Task.CompletedTask;
    }

    public Task<PostReadModel?> GetByIdAsync(Guid id)
    {
        _posts.TryGetValue(id, out var post);
        return Task.FromResult(post);
    }

    public Task<List<PostReadModel>> GetTrendingAsync(int page, int pageSize, Guid? groupId = null)
    {
        var query = _posts.Values.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        var items = query
            .OrderByDescending(p => p.Stats.TrendingScore)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(items);
    }

    public Task<List<PostReadModel>> GetLatestAsync(int page, int pageSize, Guid? groupId = null)
    {
        var query = _posts.Values.AsQueryable();

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
        var query = _posts.Values.AsQueryable();

        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        return Task.FromResult((long)query.Count());
    }
}
