// Ignore Spelling: Auth

using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Models.JwtTokens;

namespace SocialMediaApi.Interfaces
{
    public interface IAuthService
    {
        public Task<BaseUser> GetAuthenticatedUser();

        public Task<BaseUser> GetAuthorizedUser();

        public Task<BaseUser> GetImpersonatingUser();

        public Task<bool> IsAuthenticated();

        Task<TokenDto> BuildToken(AddJwtTokenModel model);
    }
}