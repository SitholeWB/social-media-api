namespace SocialMedia.Application;

public struct DefaultConstants
{
    public static List<DefaultGroupDto> DEFAULT_GROUPS =>
        [
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Group 1", GroupType.Everyone, "Group 1"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Group 2", GroupType.Everyone, "Group 2"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "Group 3", GroupType.Everyone, "Group 3"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000004"), "Group 4", GroupType.Everyone, "Group 4"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000005"), "Group 5", GroupType.Everyone, "Group 5")
        ];
}