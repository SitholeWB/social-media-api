namespace SocialMedia.Infrastructure;

public class DashboardStatsRepository(SocialMediaDbContext context) : IDashboardStatsRepository
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var totalUsers = await context.Users.CountAsync(cancellationToken);

        var activeUsers = await context.Users
            .Where(u => u.LastActiveAt >= startDate && u.LastActiveAt <= endDate)
            .CountAsync(cancellationToken);

        var totalPosts = await context.Posts.CountAsync(cancellationToken);
        var newPosts = await context.Posts
            .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
            .CountAsync(cancellationToken);

        var totalComments = await context.Comments.CountAsync(cancellationToken);
        var newComments = await context.Comments
            .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
            .CountAsync(cancellationToken);

        var totalReactions = await context.Likes.CountAsync(cancellationToken);
        var newReactions = await context.Likes
            .Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
            .CountAsync(cancellationToken);

        return new DashboardStatsDto(
            TotalUsers: totalUsers,
            ActiveUsers: activeUsers,
            TotalPosts: totalPosts,
            NewPostsInPeriod: newPosts,
            TotalComments: totalComments,
            NewCommentsInPeriod: newComments,
            TotalReactions: totalReactions,
            NewReactionsInPeriod: newReactions
        );
    }
}