namespace SocialMedia.Domain;

public class GroupActivityLevelDto
{
    public Guid? GroupId { get; set; }
    public string Name { get; set; } = "Moderate";
    public double FreshnessHalfLifeHours { get; set; }
    public double FreshnessWeight { get; set; }
    public double EngagementWeight { get; set; }
    public double NewPostBoost { get; set; }
    public double NewPostBoostDurationHours { get; set; }
    public double ReactionScale { get; set; }
    public double CommentScale { get; set; }
    public int DailyPosts { get; set; }
    public int WeeklyUsers { get; set; }
    public double EngagementRate { get; set; }
    public double AvgReactions { get; set; }
    public double AvgComments { get; set; }
    public double LatestPostAgeHours { get; set; }
}