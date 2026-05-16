namespace SocialMedia.IntegrationTests;

public struct Constants
{
    public static string DefaultGroupId => "00000000-0000-0000-0000-000000000001";
    public static string DefaultTenantId => "00000000-0000-0000-0000-000000000000";
    public static string ApiBase => $"/api/v1/{DefaultTenantId}";
}