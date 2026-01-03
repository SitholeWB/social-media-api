namespace SocialMedia.Domain;

public class GroupActivityReportDto
{
    public Guid GroupId { get; set; }
    public string ActivityLevel { get; set; }
    public int DailyPosts { get; set; }
    public int WeeklyUsers { get; set; }
    public double EngagementRate { get; set; }
    public string RecommendedAction { get; set; }
}