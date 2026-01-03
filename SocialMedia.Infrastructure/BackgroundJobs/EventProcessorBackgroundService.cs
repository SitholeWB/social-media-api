namespace SocialMedia.Infrastructure;

public class EventProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventProcessorBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);
    private DateTimeOffset _lastRun = DateTimeOffset.MinValue;
    private DateTimeOffset _lastRankingRun = DateTimeOffset.MinValue;
    private bool _run = false;

    public EventProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EventProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastRun = DateTimeOffset.UtcNow;
        if (_run)
        {
            _logger.LogInformation("Event Processor Background Service is already running.");
            return;
        }
        _logger.LogInformation("Event Processor Background Service started");
        _run = true;
        while (!stoppingToken.IsCancellationRequested && _run)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventProcessor = scope.ServiceProvider.GetRequiredService<IBackgroundEventProcessor>();
                var rankService = scope.ServiceProvider.GetRequiredService<IPostRankService>();
                var socialMediaReadDbContext = scope.ServiceProvider.GetRequiredService<SocialMediaReadDbContext>();
                var socialMediaDbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
                var postService = scope.ServiceProvider.GetRequiredService<IPostService>();

                var count = await eventProcessor.ProcessPendingEventsAsync(stoppingToken);
                if (count > 0)
                {
                    if (_lastRankingRun.AddHours(1) < DateTimeOffset.UtcNow)
                    {
                        // Update stale ranks (older than 1 hour)
                        await rankService.RecalculateAllRanksAsync(stoppingToken);
                        await MaintainPostLimitAsync(socialMediaReadDbContext, socialMediaDbContext, postService, stoppingToken);
                        _lastRankingRun = DateTimeOffset.UtcNow;
                    }
                    _lastRun = DateTimeOffset.UtcNow;
                }
                else
                {
                    if (_lastRun.AddMinutes(10) < DateTimeOffset.UtcNow)
                    {
                        _logger.LogInformation("No events processed for 10 minutes, stopping background service.");
                        _run = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending events");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Event Processor Background Service stopped");
    }

    private async Task MaintainPostLimitAsync(SocialMediaReadDbContext _context, SocialMediaDbContext _writeContext, IPostService postService, CancellationToken cancellationToken)
    {
        var groups = await _writeContext.Groups.ToListAsync(cancellationToken);
        foreach (var group in groups)
        {
            var activePosts = await _context.Posts.Where(x => x.GroupId == group.Id).CountAsync(cancellationToken);
            var _maxPosts = 20000;
            var maxBatchSize = 2000;
            if (activePosts > _maxPosts)
            {
                // Archive posts with lowest engagement and lowest scores NO assumptions about time
                // - purely based on ranking and engagement

                var postsToArchive = await _context.Posts
                    .Where(x => x.GroupId == group.Id)
                    .OrderBy(p => p.TrendingScore)           // Lowest ranking first
                    .ThenBy(p => p.ReactionCount)        // Then least reactions
                    .ThenBy(p => p.CommentCount)         // Then least comments
                    .ThenBy(p => p.CreatedAt)            // Then oldest
                    .Take(maxBatchSize)
                    .ToListAsync(cancellationToken);

                foreach (var post in postsToArchive)
                {
                    await postService.DeletePostAsync(post.Id, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted {Count} posts for groupId {groupId}", postsToArchive.Count, group.Id);
            }
        }
    }
}