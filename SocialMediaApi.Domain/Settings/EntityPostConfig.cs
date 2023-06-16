namespace SocialMediaApi.Domain.Settings
{
    public sealed class EntityPostConfig
    {
        public int PostDefaultExpireMinutes { get; set; } = 24 * 60;
    }
}