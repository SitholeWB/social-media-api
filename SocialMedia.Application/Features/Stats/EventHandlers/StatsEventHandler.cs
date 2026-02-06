using Microsoft.Extensions.Logging;

namespace SocialMedia.Application;

public class StatsEventHandler(
    IStatsRepository statsRepository,
    IUserActivityRepository userActivityRepository,
    ILogger<StatsEventHandler> logger) :
    IEventHandler<PostCreatedEvent>,
    IEventHandler<CommentAddedEvent>,
    IEventHandler<PostLikeAddedEvent>,
    IEventHandler<CommentLikeAddedEvent>
{
    public async Task HandleAsync(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateStatsAsync(notification.Post.CreatedAt,
            record => record.NewPosts++,
            record => record.TotalPosts++,
            notification.Post.AuthorId,
            cancellationToken);
    }

    public async Task HandleAsync(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateStatsAsync(notification.Comment.CreatedAt,
            record => record.ResultingComments++,
            null,
            notification.Comment.AuthorId,
            cancellationToken);
    }

    public async Task HandleAsync(PostLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateReactionStatsAsync(DateTimeOffset.UtcNow, notification.Like.UserId, notification.Like.Emoji, cancellationToken);
    }

    public async Task HandleAsync(CommentLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateReactionStatsAsync(DateTimeOffset.UtcNow, notification.Like.UserId, notification.Like.Emoji, cancellationToken);
    }

    private async Task UpdateStatsAsync(DateTimeOffset activityDate, Action<StatsRecord> updateAction, Action<StatsRecord>? globalCounterAction, Guid userId, CancellationToken cancellationToken)
    {
        await UpdatePeriodStatsAsync(StatsType.Weekly, activityDate.ToWeekStartDate(), updateAction, globalCounterAction, userId, cancellationToken);
        await UpdatePeriodStatsAsync(StatsType.Monthly, activityDate.ToMonthStartDate(), updateAction, globalCounterAction, userId, cancellationToken);
    }

    private async Task UpdateReactionStatsAsync(DateTimeOffset activityDate, Guid userId, string emoji, CancellationToken cancellationToken)
    {
        await UpdatePeriodReactionStatsAsync(StatsType.Weekly, activityDate.ToWeekStartDate(), userId, emoji, cancellationToken);
        await UpdatePeriodReactionStatsAsync(StatsType.Monthly, activityDate.ToMonthStartDate(), userId, emoji, cancellationToken);
    }

    private async Task UpdatePeriodStatsAsync(StatsType type, DateTimeOffset startDate, Action<StatsRecord> updateAction, Action<StatsRecord>? globalCounterAction, Guid userId, CancellationToken cancellationToken)
    {
        var record = await GetOrCreateStatsRecordAsync(type, startDate, cancellationToken);

        updateAction(record);
        globalCounterAction?.Invoke(record);

        await UpdateActiveUsersAsync(record, userId, startDate, cancellationToken);

        await statsRepository.UpdateAsync(record, cancellationToken);
    }

    private async Task UpdatePeriodReactionStatsAsync(StatsType type, DateTimeOffset startDate, Guid userId, string emoji, CancellationToken cancellationToken)
    {
        var record = await GetOrCreateStatsRecordAsync(type, startDate, cancellationToken);

        record.ResultingReactions++;

        var reactionStat = record.ReactionBreakdown.FirstOrDefault(r => r.Emoji == emoji);
        if (reactionStat == null)
        {
            reactionStat = new ReactionStat { Emoji = emoji, Count = 0 };
            record.ReactionBreakdown.Add(reactionStat);
        }
        reactionStat.Count++;

        await UpdateActiveUsersAsync(record, userId, startDate, cancellationToken);

        await statsRepository.UpdateAsync(record, cancellationToken);
    }

    private async Task UpdateActiveUsersAsync(StatsRecord record, Guid userId, DateTimeOffset startDate, CancellationToken cancellationToken)
    {
        // Simple heuristic: If User's LastActiveAt is OLDER than the start of this period, it means
        // this is their FIRST action in this period. HOWEVER, UserActivityEventHandler updates
        // LastActiveAt roughly at the same time. So we need to be careful. Better approach: Check
        // if we have ALREADY counted this user? No, we don't store a list of users in StatsRecord.
        // Alternative: Query UserActivityRepository to see if they were active in this period
        // BEFORE this current event? But this current event IS the activity.

        // Let's rely on the UserActivityRepository. If the user's LastActiveAt (before this update)
        // was < StartDate, then they are "newly active" for this period. But we don't know the
        // "before" state here easily without fetching User first.

        // Optimization: Fetch UserActivity.
        var userActivity = await userActivityRepository.GetByUserIdAsync(userId, true, cancellationToken);
        var lastActive = userActivity?.LastSeenAt ?? DateTimeOffset.MinValue;

        // If their LAST recorded activity was BEFORE the start of this period, then they are
        // becoming active now.
        if (lastActive < startDate)
        {
            record.ActiveUsers++;
            await userActivityRepository.UpdateUserLastSeenAsync(userId, cancellationToken);
        }
    }

    private async Task<StatsRecord> GetOrCreateStatsRecordAsync(StatsType type, DateTimeOffset startDate, CancellationToken cancellationToken)
    {
        var date = type == StatsType.Weekly ? startDate.ToWeekStartDate() : startDate.ToMonthStartDate();
        var record = await statsRepository.GetCurrentStatsRecordAsync(type, date, cancellationToken);
        if (record == null)
        {
            record = new StatsRecord
            {
                StatsType = type,
                Date = date,
                ReactionBreakdown = new List<ReactionStat>()
            };
            await statsRepository.AddAsync(record, cancellationToken);
        }
        return record;
    }
}