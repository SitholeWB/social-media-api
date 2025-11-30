using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Domain.Interfaces;

public interface ICommentReadRepository
{
    Task AddAsync(CommentReadModel comment);
    Task UpdateAsync(CommentReadModel comment);
    Task<CommentReadModel?> GetByIdAsync(Guid id);
    Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize);
}
