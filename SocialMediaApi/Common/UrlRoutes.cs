namespace SocialMediaApi.Common
{
    public struct UrlRoutes
    {
        private const string VERSION_1 = "v1";
        public const string GetFeed = $"/api/{VERSION_1}/feed";
        public static string GetTrendingFeed => $"/api/{VERSION_1}/feed/trending";
    }
}