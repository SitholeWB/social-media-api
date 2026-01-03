namespace SocialMedia.Application;

public interface IPostRankService
{
    Task<double> CalculatePostRankAsync(PostReadModel post, CancellationToken cancellationToken);

    Task<List<GroupActivityReportDto>> GetGroupActivityReportsAsync(CancellationToken cancellationToken);

    Task<List<PostReadModel>> GetRankedPostsAsync(Guid? groupId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    Task RecalculateAllRanksAsync(CancellationToken cancellationToken);

    Task UpdatePostRankAsync(Guid postId, CancellationToken cancellationToken);
}