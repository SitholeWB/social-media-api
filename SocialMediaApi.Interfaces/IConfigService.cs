using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Interfaces
{
    public interface IConfigService
    {
        public Task<int> GetExpireDateMinutesConfig(EntityActionType entityActionType);

        public Task<int> GetRankingConfig(EntityActionType entityActionType);
    }
}