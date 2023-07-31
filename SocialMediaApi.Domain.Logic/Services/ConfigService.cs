using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Settings;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.Services
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
                RankIncrement = 1,
                ExpireDateMinutes = 15
            });
        }

        public async Task<EntityPostConfig> GetEntityPostConfigAsync()
        {
            return await Task.FromResult(new EntityPostConfig
            {
            });
        }

        public async Task<JwtConfig> GetJwtConfigAsync()
        {
            return await Task.FromResult(new JwtConfig
            {
            });
        }
    }
}