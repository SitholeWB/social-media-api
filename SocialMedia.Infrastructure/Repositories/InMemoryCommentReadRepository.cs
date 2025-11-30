using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Infrastructure.Repositories;

public class InMemoryCommentReadRepository : ICommentReadRepository
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, CommentReadModel> _comments = new();

    public Task AddAsync(CommentReadModel comment)
    {
        _comments.TryAdd(comment.Id, comment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CommentReadModel comment)
    {
        _comments.AddOrUpdate(comment.Id, comment, (key, oldValue) => comment);
        return Task.CompletedTask;
    }

    public Task<CommentReadModel?> GetByIdAsync(Guid id)
    {
        _comments.TryGetValue(id, out var comment);
        return Task.FromResult(comment);
    }

    public Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize)
    {
        var comments = _comments.Values
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return Task.FromResult(comments);
    }
}
