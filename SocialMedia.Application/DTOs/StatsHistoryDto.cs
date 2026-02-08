namespace SocialMedia.Application;

public class StatsHistoryDto
{
    public List<StatsRecord> Weeks { get; set; } = [];
    public List<StatsRecord> Months { get; set; } = [];
}