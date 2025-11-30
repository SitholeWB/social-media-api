namespace SocialMedia.Application;

public class GetActivePollsQueryHandler : IQueryHandler<GetActivePollsQuery, PagedResult<PollDto>>
{
    private readonly IPollRepository _pollRepository;

    public GetActivePollsQueryHandler(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<PagedResult<PollDto>> Handle(GetActivePollsQuery query, CancellationToken cancellationToken)
    {
        var (polls, totalCount) = await _pollRepository.GetActivePollsPagedAsync(query.PageNumber, query.PageSize, cancellationToken);
        var dtos = polls.Select(p => p.ToDto()).ToList();
        return new PagedResult<PollDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}
