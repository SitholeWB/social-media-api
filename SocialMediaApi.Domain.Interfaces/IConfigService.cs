﻿using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Settings;

namespace SocialMediaApi.Domain.Interfaces
{
    public interface IConfigService
    {
        public Task<EntityActionConfig> GetActionConfigAsync(EntityActionType entityActionType);

        public Task<EntityPostConfig> GetEntityPostConfigAsync();

        public Task<JwtConfig> GetJwtConfigAsync();
    }
}