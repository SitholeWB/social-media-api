namespace SocialMedia.Application;

public class GetPollQueryHandler : IQueryHandler<GetPollQuery, PollDto?>
{
    private readonly IPollRepository _pollRepository;

    public GetPollQueryHandler(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<PollDto?> Handle(GetPollQuery query, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(query.PollId, cancellationToken);
        if (poll == null)
        {
            return null;
        }

        return poll.ToDto();
    }
}
