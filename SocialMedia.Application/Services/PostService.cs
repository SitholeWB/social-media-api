namespace SocialMedia.Application;

public class PostService : IPostService
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;
    private readonly IPostVectorService _postVectorService;

    public PostService(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository,
        IPostVectorService postVectorService)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
        _postVectorService = postVectorService;
    }

    public async Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);
        if (post != null)
        {
            var deleted = await _readRepository.DeleteByIdAsync(post.Id, cancellationToken);
            if (deleted)
            {
                await _postVectorService.RemovePostEmbeddingAsync(id, cancellationToken);
                var comments = new List<CommentReadModel>();
                do
                {
                    comments = await _commentReadRepository.GetByPostIdAsync(post.Id, 1, 100, cancellationToken);
                    foreach (var comment in comments)
                    {
                        await _commentReadRepository.DeleteByIdAsync(comment.Id, cancellationToken);
                    }
                } while (comments.Count > 0);
            }
            return deleted;
        }
        return false;
    }
}