using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.Settings
{
    public sealed class EntityRankingConfig
    {
        public IDictionary<EntityActionType, int> Values { get; set; } = new Dictionary<EntityActionType, int>();
    }
}