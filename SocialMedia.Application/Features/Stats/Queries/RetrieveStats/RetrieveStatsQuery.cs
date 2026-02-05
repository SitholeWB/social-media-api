using SocialMedia.Domain;

namespace SocialMedia.Application;

public record RetrieveStatsQuery(StatsType Type, DateTimeOffset Date) : IQuery<StatsRecord?>;