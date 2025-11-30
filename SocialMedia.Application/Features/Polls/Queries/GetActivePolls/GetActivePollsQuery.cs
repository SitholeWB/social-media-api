namespace SocialMedia.Application;

public record GetActivePollsQuery(int PageNumber, int PageSize) : IQuery<PagedResult<PollDto>>;
