namespace SocialMedia.Application;

public class DeleteReportedContentCommandHandler : ICommandHandler<DeleteReportedContentCommand, int>
{
    private readonly IReportRepository _reportRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public DeleteReportedContentCommandHandler(
        IReportRepository reportRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        _reportRepository = reportRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task<int> Handle(DeleteReportedContentCommand request, CancellationToken cancellationToken)
    {
        int deletedCount = 0;

        // Find posts with > MinReports
        var postsToDelete = await _reportRepository.GetPostIdsWithReportsAboveThresholdAsync(request.MinReports, cancellationToken);

        foreach (var postId in postsToDelete)
        {
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
            if (post != null)
            {
                await _postRepository.DeleteAsync(post, cancellationToken);
                deletedCount++;
            }
        }

        // Find comments with > MinReports
        var commentsToDelete = await _reportRepository.GetCommentIdsWithReportsAboveThresholdAsync(request.MinReports, cancellationToken);

        foreach (var commentId in commentsToDelete)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId, cancellationToken);
            if (comment != null)
            {
                await _commentRepository.DeleteAsync(comment, cancellationToken);
                deletedCount++;
            }
        }

        return deletedCount;
    }
}