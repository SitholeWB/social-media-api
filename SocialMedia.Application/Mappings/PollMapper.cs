namespace SocialMedia.Application;

public static class PollMapper
{
    public static PollDto ToDto(this Poll poll, UserActivity? userActivity = null)
    {
        var vote = userActivity?.Votes.FirstOrDefault(v => v.PollId == poll.Id);

        return new PollDto
        {
            Id = poll.Id,
            Question = poll.Question,
            IsActive = poll.IsActive,
            ExpiresAt = poll.ExpiresAt,
            CreatorId = poll.CreatorId,
            GroupId = poll.GroupId,
            GroupName = poll.Group?.Name ?? string.Empty,
            IsAnonymous = poll.IsAnonymous,
            HasVoted = vote != null,
            VotedOptionId = vote?.OptionId,
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