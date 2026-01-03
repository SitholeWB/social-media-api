namespace SocialMedia.Infrastructure;

public class AutoScalingPostRankService : IPostRankService
{
    private readonly SocialMediaReadDbContext _readDbContext;
    private readonly SocialMediaDbContext _writeDbContext;
    private readonly ILogger<AutoScalingPostRankService> _logger;
    private readonly IDistributedCache _activityCache;
    private const int CACHE_DURATION_MINUTES = 30;

    public AutoScalingPostRankService(
        SocialMediaReadDbContext readDbContext,
        ILogger<AutoScalingPostRankService> logger,
        SocialMediaDbContext writeDbContext,
        IDistributedCache cache)
    {
        _readDbContext = readDbContext;
        _logger = logger;
        _writeDbContext = writeDbContext;
        _activityCache = cache;
    }

    public async Task<double> CalculatePostRankAsync(PostReadModel post, CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;
        var groupActivity = await GetGroupActivityLevelAsync(post.GroupId, cancellationToken);
        return await CalculatePostRankAsync(post, currentTime, groupActivity, cancellationToken);
    }

    private async Task<double> CalculatePostRankAsync(PostReadModel post, DateTime currentTime, GroupActivityLevelDto activity, CancellationToken cancellationToken)
    {
        double hoursOld = (currentTime - post.CreatedAt).TotalHours;

        // 1. Engagement with group-specific scaling
        double engagementScore = CalculateEngagementScore(post, activity);

        // 2. Freshness with group-specific decay
        double freshnessScore = Math.Pow(0.5, hoursOld / activity.FreshnessHalfLifeHours);

        // 3. Base rank
        double rank = (engagementScore * activity.EngagementWeight) +
                     (freshnessScore * activity.FreshnessWeight);

        // 4. New post boost (group-specific)
        if (hoursOld < activity.NewPostBoostDurationHours)
        {
            double boostDecay = 1.0 - (hoursOld / activity.NewPostBoostDurationHours);
            rank *= (1 + (activity.NewPostBoost - 1) * boostDecay);
        }

        // 5. Quick engagement bonus
        if (hoursOld < 12 && (post.ReactionCount > 0 || post.CommentCount > 0))
        {
            var firstEngagement = await GetFirstEngagementTimeAsync(post.Id, cancellationToken);
            if (firstEngagement.HasValue)
            {
                var hoursToEngagement = (firstEngagement.Value - post.CreatedAt).TotalHours;
                if (hoursToEngagement < 3) rank *= 1.3;
                else if (hoursToEngagement < 6) rank *= 1.2;
            }
        }

        // 6. Group-specific adjustments
        rank = ApplyGroupSpecificAdjustments(rank, post, activity);

        return Math.Max(0.01, Math.Min(rank, 10.0));
    }

    private async Task<GroupActivityLevelDto> GetGroupActivityLevelAsync(Guid? groupId, CancellationToken cancellationToken)
    {
        var cacheKey = $"GroupActivity_{groupId}";

        //if (_activityCache.TryGetValue(cacheKey, out GroupActivityLevelDto cachedActivity))
        //{
        //    return cachedActivity;
        //}

        var activity = await CalculateGroupActivityLevelAsync(groupId.Value, cancellationToken);
        // _activityCache.Set(cacheKey, activity, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return activity;
    }

    private async Task<GroupActivityLevelDto> CalculateGroupActivityLevelAsync(Guid groupId, CancellationToken cancellationToken)
    {
        try
        {
            // Get group-specific metrics
            var groupMetrics = await _readDbContext.Posts
                .Where(p => p.GroupId == groupId)
                .GroupBy(p => 1)
                .Select(g => new
                {
                    TotalPosts = g.Count(),
                    DailyPosts = g.Count(p => p.CreatedAt > DateTime.UtcNow.AddDays(-1)),
                    WeeklyPosts = g.Count(p => p.CreatedAt > DateTime.UtcNow.AddDays(-7)),
                    WeeklyUsers = g.Select(p => p.AuthorId).Distinct().Count(),
                    AvgReactions = g.Average(p => (double)p.ReactionCount),
                    AvgComments = g.Average(p => (double)p.CommentCount),
                    LatestPost = g.Max(p => p.CreatedAt)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (groupMetrics == null)
            {
                // New or inactive group - use conservative defaults
                return CreateGroupActivityLevel("New", groupId, 0, 0, 0);
            }

            // Calculate engagement rate for this group
            var engagedPosts = await _readDbContext.Posts
                .CountAsync(p => p.GroupId == groupId &&
                                 p.CreatedAt > DateTime.UtcNow.AddDays(-7) &&
                                (p.ReactionCount > 0 || p.CommentCount > 0));

            double engagementRate = groupMetrics.WeeklyPosts > 0
                ? (double)engagedPosts / groupMetrics.WeeklyPosts
                : 0;

            // Determine activity level for this specific group
            string activityLevel = DetermineGroupActivityLevel(
                groupMetrics.DailyPosts,
                groupMetrics.WeeklyUsers,
                engagementRate,
                groupMetrics.AvgReactions);

            var activity = CreateGroupActivityLevel(
                activityLevel,
                groupId,
                groupMetrics.DailyPosts,
                groupMetrics.WeeklyUsers,
                engagementRate);

            // Store group-specific metrics
            activity.GroupId = groupId;
            activity.DailyPosts = groupMetrics.DailyPosts;
            activity.WeeklyUsers = groupMetrics.WeeklyUsers;
            activity.EngagementRate = engagementRate;
            activity.AvgReactions = groupMetrics.AvgReactions;
            activity.AvgComments = groupMetrics.AvgComments;
            var totalHours = (DateTime.UtcNow - groupMetrics.LatestPost).TotalHours;
            activity.LatestPostAgeHours = totalHours > 0 ? totalHours : 24 * 30; // 30 days if no posts

            return activity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating activity for group {GroupId}", groupId);
            return CreateGroupActivityLevel("Moderate", groupId, 10, 20, 0.3);
        }
    }

    private string DetermineGroupActivityLevel(int dailyPosts, int weeklyUsers, double engagementRate, double avgReactions)
    {
        // Multi-dimensional determination
        if (dailyPosts < 1 && weeklyUsers < 3)
            return "Inactive";

        if (dailyPosts < 3 && weeklyUsers < 5)
            return "VeryQuiet";

        if (dailyPosts < 10 && weeklyUsers < 15)
        {
            // Check engagement quality
            if (engagementRate > 0.6 || avgReactions > 2)
                return "QuietEngaged";
            return "Quiet";
        }

        if (dailyPosts < 30 && weeklyUsers < 50)
        {
            if (engagementRate > 0.7)
                return "ModerateEngaged";
            return "Moderate";
        }

        if (dailyPosts < 100 && weeklyUsers < 150)
            return "Active";

        if (dailyPosts < 300 && weeklyUsers < 500)
            return "VeryActive";

        return "HyperActive";
    }

    private GroupActivityLevelDto CreateGroupActivityLevel(
        string level, Guid groupId, int dailyPosts, int weeklyUsers, double engagementRate)
    {
        var activity = level switch
        {
            "Inactive" => new GroupActivityLevelDto
            {
                Name = "Inactive",
                FreshnessHalfLifeHours = 720,      // 30 days - very slow decay
                FreshnessWeight = 0.2,
                EngagementWeight = 0.8,
                NewPostBoost = 3.0,
                NewPostBoostDurationHours = 168,   // 1 week boost
                ReactionScale = 2,
                CommentScale = 1
            },
            "VeryQuiet" => new GroupActivityLevelDto
            {
                Name = "VeryQuiet",
                FreshnessHalfLifeHours = 336,      // 2 weeks
                FreshnessWeight = 0.25,
                EngagementWeight = 0.75,
                NewPostBoost = 2.5,
                NewPostBoostDurationHours = 96,    // 4 days
                ReactionScale = 3,
                CommentScale = 2
            },
            "Quiet" => new GroupActivityLevelDto
            {
                Name = "Quiet",
                FreshnessHalfLifeHours = 168,      // 1 week
                FreshnessWeight = 0.35,
                EngagementWeight = 0.65,
                NewPostBoost = 2.0,
                NewPostBoostDurationHours = 72,    // 3 days
                ReactionScale = 5,
                CommentScale = 3
            },
            "QuietEngaged" => new GroupActivityLevelDto
            {
                Name = "QuietEngaged",
                FreshnessHalfLifeHours = 120,      // 5 days
                FreshnessWeight = 0.4,
                EngagementWeight = 0.6,
                NewPostBoost = 1.8,
                NewPostBoostDurationHours = 48,    // 2 days
                ReactionScale = 4,
                CommentScale = 2
            },
            "Moderate" => new GroupActivityLevelDto
            {
                Name = "Moderate",
                FreshnessHalfLifeHours = 96,       // 4 days
                FreshnessWeight = 0.45,
                EngagementWeight = 0.55,
                NewPostBoost = 1.6,
                NewPostBoostDurationHours = 36,    // 1.5 days
                ReactionScale = 8,
                CommentScale = 4
            },
            "ModerateEngaged" => new GroupActivityLevelDto
            {
                Name = "ModerateEngaged",
                FreshnessHalfLifeHours = 72,       // 3 days
                FreshnessWeight = 0.5,
                EngagementWeight = 0.5,
                NewPostBoost = 1.5,
                NewPostBoostDurationHours = 24,    // 1 day
                ReactionScale = 6,
                CommentScale = 3
            },
            "Active" => new GroupActivityLevelDto
            {
                Name = "Active",
                FreshnessHalfLifeHours = 48,       // 2 days
                FreshnessWeight = 0.55,
                EngagementWeight = 0.45,
                NewPostBoost = 1.4,
                NewPostBoostDurationHours = 24,
                ReactionScale = 15,
                CommentScale = 8
            },
            "VeryActive" => new GroupActivityLevelDto
            {
                Name = "VeryActive",
                FreshnessHalfLifeHours = 36,       // 1.5 days
                FreshnessWeight = 0.6,
                EngagementWeight = 0.4,
                NewPostBoost = 1.3,
                NewPostBoostDurationHours = 18,
                ReactionScale = 25,
                CommentScale = 12
            },
            "HyperActive" => new GroupActivityLevelDto
            {
                Name = "HyperActive",
                FreshnessHalfLifeHours = 24,       // 1 day
                FreshnessWeight = 0.65,
                EngagementWeight = 0.35,
                NewPostBoost = 1.2,
                NewPostBoostDurationHours = 12,
                ReactionScale = 40,
                CommentScale = 20
            },
            _ => CreateGroupActivityLevel("Moderate", groupId, dailyPosts, weeklyUsers, engagementRate)
        };

        activity.GroupId = groupId;
        activity.DailyPosts = dailyPosts;
        activity.WeeklyUsers = weeklyUsers;
        activity.EngagementRate = engagementRate;

        return activity;
    }

    private double CalculateEngagementScore(PostReadModel post, GroupActivityLevelDto activity)
    {
        // Group-specific engagement scaling
        double reactionScore = Math.Min(post.ReactionCount / activity.ReactionScale, 1.0);
        double commentScore = Math.Min(post.CommentCount / activity.CommentScale, 1.0);

        // Engagement bonuses adjusted by group activity
        double diversityBonus = (post.ReactionCount > 0 && post.CommentCount > 0) ? 0.15 : 0;

        // Inactive groups, any engagement is extra valuable
        double anyEngagementBonus = (post.ReactionCount > 0 || post.CommentCount > 0)
            ? (activity.Name == "Inactive" ? 0.3 : 0.1)
            : 0;

        return (reactionScore * 0.7) + (commentScore * 0.3) + diversityBonus + anyEngagementBonus;
    }

    private double ApplyGroupSpecificAdjustments(double rank, PostReadModel post, GroupActivityLevelDto activity)
    {
        // Boost posts in inactive groups to encourage activity
        if (activity.Name == "Inactive" || activity.Name == "VeryQuiet")
        {
            rank *= 1.5;
        }

        // Slight boost for posts in highly engaged groups
        if (activity.EngagementRate > 0.7)
        {
            rank *= 1.1;
        }

        // If group has been inactive, new posts get extra boost
        if (activity.LatestPostAgeHours > 168 && post.CreatedAt > DateTime.UtcNow.AddHours(-24))
        {
            rank *= 1.8; // Big boost to revive inactive group
        }

        return rank;
    }

    public async Task<List<PostReadModel>> GetRankedPostsAsync(Guid? groupId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;
        var query = _readDbContext.Posts.AsNoTracking();

        // Filter by group if specified
        if (groupId.HasValue)
        {
            query = query.Where(p => p.GroupId == groupId);
        }

        // Calculate scores and order
        var rankedPosts = await query
            .OrderByDescending(x => x.TrendingScore)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return rankedPosts;
    }

    public async Task UpdatePostRankAsync(Guid postId, CancellationToken cancellationToken)
    {
        var post = await _readDbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (post != null)
        {
            var groupActivity = await GetGroupActivityLevelAsync(post.GroupId, cancellationToken);
            post.TrendingScore = await CalculatePostRankAsync(post, DateTime.UtcNow, groupActivity, cancellationToken);
            post.RankUpdatedAt = DateTime.UtcNow;
            await _readDbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache for this group
            InvalidateGroupCache(post.GroupId);
        }
    }

    public async Task RecalculateAllRanksAsync(CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;

        // Process in batches for performance
        const int batchSize = 100;
        var totalProcessed = 0;

        var postIds = await _readDbContext.Posts
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        foreach (var batch in postIds.Chunk(batchSize))
        {
            foreach (var postId in batch)
            {
                var post = await _readDbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

                if (post != null)
                {
                    var groupActivity = await GetGroupActivityLevelAsync(post.GroupId, cancellationToken);
                    post.TrendingScore = await CalculatePostRankAsync(post, currentTime, groupActivity, cancellationToken);
                    post.RankUpdatedAt = currentTime;
                }
            }

            await _readDbContext.SaveChangesAsync(cancellationToken);
            totalProcessed += batch.Length;
            _logger.LogInformation("Recalculated ranks for {Count} posts", totalProcessed);
        }

        // Clear all cache
        //_activityCache.Clear();
    }

    public async Task<List<GroupActivityReportDto>> GetGroupActivityReportsAsync(CancellationToken cancellationToken)
    {
        var groupIds = await _readDbContext.Posts
            .Select(p => p.GroupId)
            .Distinct()
            .Where(g => g.HasValue)
            .Select(g => g.Value)
            .ToListAsync(cancellationToken);

        var reports = new List<GroupActivityReportDto>();

        foreach (var groupId in groupIds)
        {
            var activity = await GetGroupActivityLevelAsync(groupId, cancellationToken);
            reports.Add(new GroupActivityReportDto
            {
                GroupId = groupId,
                ActivityLevel = activity.Name,
                DailyPosts = activity.DailyPosts,
                WeeklyUsers = activity.WeeklyUsers,
                EngagementRate = activity.EngagementRate,
                RecommendedAction = GetRecommendedAction(activity)
            });
        }

        return reports.OrderByDescending(r => r.DailyPosts).ToList();
    }

    private string GetRecommendedAction(GroupActivityLevelDto activity)
    {
        return activity.Name switch
        {
            "Inactive" => "Consider promoting this group or merging with another",
            "VeryQuiet" => "Encourage members to post more frequently",
            "Quiet" => "Continue current strategy, engagement is good",
            "Moderate" => "Group is healthy, no action needed",
            "Active" => "Consider creating subgroups if growth continues",
            "VeryActive" => "Monitor for spam/quality issues",
            "HyperActive" => "May need content moderation tools",
            _ => "Monitor group activity"
        };
    }

    private async Task<DateTimeOffset?> GetFirstEngagementTimeAsync(Guid postId, CancellationToken cancellationToken)
    {
        var firstReaction = await _writeDbContext.Reactions
            .Where(r => r.PostId == postId)
            .OrderBy(r => r.CreatedAt)
            .Select(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var firstComment = await _readDbContext.Comments
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (firstReaction != default && firstComment != default)
            return firstReaction < firstComment ? firstReaction : firstComment;

        return firstReaction == default ? firstReaction : firstReaction;
    }

    private string DetermineGlobalActivityLevel(int dailyPosts, int weeklyUsers)
    {
        if (dailyPosts < 10) return "Quiet";
        if (dailyPosts < 50) return "Moderate";
        if (dailyPosts < 200) return "Active";
        return "VeryActive";
    }

    private void InvalidateGroupCache(Guid? groupId)
    {
        if (groupId.HasValue)
        {
            _activityCache.Remove($"GroupActivity_{groupId}");
        }
    }
}