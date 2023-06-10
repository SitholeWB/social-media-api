namespace SocialMediaApi.Domain.Settings
{
    public sealed class EntityGroupPostConfig
    {
        public int GroupPostDefaultExpireMinutes { get; set; } = 24 * 60;
    }
}