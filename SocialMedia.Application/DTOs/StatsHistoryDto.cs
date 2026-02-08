namespace SocialMedia.Application;

public class StatsHistoryDto
{
    public List<StatsRecord> Weekly { get; set; } = [];
    public List<StatsRecord> Monthly { get; set; } = [];
}