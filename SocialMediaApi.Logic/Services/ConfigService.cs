using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Settings;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class ConfigService : IConfigService
    {
        private static IDictionary<EntityActionType, EntityActionConfig> entityActions = new Dictionary<EntityActionType, EntityActionConfig>();

        public async Task<EntityActionConfig> GetActionConfigAsync(EntityActionType entityActionType)
        {
            if (entityActions.ContainsKey(entityActionType))
            {
                return await Task.FromResult(entityActions[entityActionType]);
            }
            return await Task.FromResult(new EntityActionConfig
            {
                Rank = 1,
                ExpireDateMinutes = 15
            });
        }
    }
}