using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Services
{
    public class AuthService : IAuthService
    {
        public async Task<BaseUser> GetAuthenticatedUser()
        {
            return await Task.FromResult(new BaseUser
            {
                ImageUrl = "To be implemented.",
                Name = "To be implemented.",
                Role = "To be implemented.",
            });
        }

        public async Task<BaseUser> GetAuthorizedUser()
        {
            return await Task.FromResult(new BaseUser
            {
                ImageUrl = "To be implemented.",
                Name = "To be implemented.",
                Role = "To be implemented.",
            });
        }

        public async Task<BaseUser> GetImpersonatingUser()
        {
            return await Task.FromResult(new BaseUser
            {
                ImageUrl = "To be implemented.",
                Name = "To be implemented.",
                Role = "To be implemented.",
            });
        }
    }
}