namespace SocialMedia.Application;

public record DashboardStatsDto(
    int TotalUsers,
    int ActiveUsers,
    int TotalPosts,
    int NewPostsInPeriod,
    int TotalComments,
    int NewCommentsInPeriod,
    int TotalReactions,
    int NewReactionsInPeriod
);