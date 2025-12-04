namespace SocialMedia.Application;

public record GetGroupQuery(Guid GroupId) : IQuery<GroupDto?>;
