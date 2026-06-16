namespace SocialMedia.Application;

public struct DefaultConstants
{
    public static List<DefaultGroupDto> DEFAULT_GROUPS =>
        [
            new(Guid.Parse("D0000000-0000-0000-0000-000000000001"), "Home", GroupType.Everyone, "Home Page"),
            new(Guid.Parse("D0000000-0000-0000-0000-000000000002"), "About", GroupType.Everyone, "About Page"),
            new(Guid.Parse("D0000000-0000-0000-0000-000000000003"), "Group 2", GroupType.Everyone, "Group 2"),
            new(Guid.Parse("D0000000-0000-0000-0000-000000000004"), "Group 3", GroupType.Everyone, "Group 3"),
            new(Guid.Parse("D0000000-0000-0000-0000-000000000005"), "Group 4", GroupType.Everyone, "Group 4"),
            new(Guid.Parse("D0000000-0000-0000-0000-000000000006"), "Group 5", GroupType.Everyone, "Group 5")
        ];

    public static List<DefaultTenantDto> DEFAULT_TENANTS =>
        [
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "AmaZulu FC", "AmaZulu FC Tenant"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Kaizer Chiefs FC", "Kaizer Chiefs FC Tenant"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "Orlando Pirates FC", "Orlando Pirates FC Tenant"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000004"), "Golden Arrows FC", "Golden Arrows FC Tenant")
        ];
}