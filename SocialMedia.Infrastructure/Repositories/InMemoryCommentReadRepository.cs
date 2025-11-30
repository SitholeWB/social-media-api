using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Infrastructure.Repositories;

public class InMemoryCommentReadRepository : ICommentReadRepository
{
    private readonly List<CommentReadModel> _comments = new();

    public Task AddAsync(CommentReadModel comment)
    {
        _comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CommentReadModel comment)
    {
        var existing = _comments.FirstOrDefault(c => c.Id == comment.Id);
        if (existing != null)
        {
            _comments.Remove(existing);
            _comments.Add(comment);
        }
        return Task.CompletedTask;
    }

    public Task<CommentReadModel?> GetByIdAsync(Guid id)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(comment);
    }

    public Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize)
    {
        var comments = _comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return Task.FromResult(comments);
    }
}
