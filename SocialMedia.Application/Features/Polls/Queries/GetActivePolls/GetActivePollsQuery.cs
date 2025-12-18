namespace SocialMedia.Application;

public record GetActivePollsQuery(Guid? GroupId, int PageNumber, int PageSize) : IQuery<PagedResult<PollDto>>
{
    public Guid? UserId { get; set; }
}
