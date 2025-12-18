
namespace SocialMedia.Application;

public static class PollMapper
{
    public static PollDto ToDto(this Poll poll)
    {
        return new PollDto
        {
            Id = poll.Id,
            Question = poll.Question,
            IsActive = poll.IsActive,
            ExpiresAt = poll.ExpiresAt,
            CreatorId = poll.CreatorId,
            GroupId = poll.GroupId,
            IsAnonymous = poll.IsAnonymous,
            Options = poll.Options.Select(o => o.ToDto()).ToList()
        };
    }

    public static PollOptionDto ToDto(this PollOption option)
    {
        return new PollOptionDto
        {
            Id = option.Id,
            Text = option.Text,
            VoteCount = option.Votes.Count
        };
    }
}
