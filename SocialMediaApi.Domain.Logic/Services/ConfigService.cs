using Microsoft.Extensions.Options;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Settings;

namespace SocialMediaApi.Domain.Logic.Services
{
	public class ConfigService : IConfigService
	{
		private static IDictionary<EntityActionType, EntityActionConfig> entityActions = new Dictionary<EntityActionType, EntityActionConfig>();
		private readonly IOptions<JwtConfig> _jwtConfig;

		public ConfigService(IOptions<JwtConfig> jwtConfig)
		{
			_jwtConfig = jwtConfig;
		}

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
			return await Task.FromResult(_jwtConfig?.Value ?? new JwtConfig { });
		}
	}
}