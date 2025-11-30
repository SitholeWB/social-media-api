namespace SocialMedia.Application;

public record GetPollQuery(Guid PollId) : IQuery<PollDto?>;
