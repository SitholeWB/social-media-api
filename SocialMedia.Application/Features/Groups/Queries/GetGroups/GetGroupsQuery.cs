namespace SocialMedia.Application;

public record GetGroupsQuery(int PageNumber = 1, int PageSize = 10, bool IncludeDefaults = false) : IQuery<PagedResult<GroupDto>>;