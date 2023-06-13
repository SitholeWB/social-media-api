namespace SocialMediaApi.Domain.Settings
{
    public sealed class EntityActionConfig
    {
        public int RankIncrement { get; set; } = 0;
        public int ExpireDateMinutes { get; set; } = 0;
    }
}