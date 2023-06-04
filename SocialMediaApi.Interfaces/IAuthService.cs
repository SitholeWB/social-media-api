// Ignore Spelling: Auth

using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Interfaces
{
    public interface IAuthService
    {
        public Task<BaseUser> GetAuthenticatedUser();

        public Task<BaseUser> GetAuthorizedUser();

        public Task<BaseUser> GetImpersonatingUser();
    }
}