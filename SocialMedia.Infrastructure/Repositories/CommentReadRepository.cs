using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Interfaces;
using SocialMedia.Domain.ReadModels;
using SocialMedia.Infrastructure.Persistence;

namespace SocialMedia.Infrastructure.Repositories;

public class CommentReadRepository : ICommentReadRepository
{
    private readonly SocialMediaReadDbContext _context;

    public CommentReadRepository(SocialMediaReadDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CommentReadModel comment, CancellationToken cancellationToken = default)
    {
        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CommentReadModel comment, CancellationToken cancellationToken = default)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CommentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Comments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
