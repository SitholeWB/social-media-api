using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Extensions
{
    public static class UserExtensions
    {
        public static string GetId(this UserPost _, Guid userId, int page, string subKey)
        {
            return $"{userId}_{page * 100}_{subKey}".ToLowerInvariant();
        }
    }
}