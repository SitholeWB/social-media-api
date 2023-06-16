namespace SocialMediaApi.Domain.Common
{
    public struct GenerateKeys
    {
        public static string GetUserPostId(Guid userId, int page, string subKey) => $"{userId}_{page * 100}_{subKey}".ToLowerInvariant();
    }
}