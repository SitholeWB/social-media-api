namespace SocialMedia.Application;

public class SubmitFeedbackCommandHandler : ICommandHandler<SubmitFeedbackCommand, bool>
{
    private readonly IRepository<Feedback> _feedbackRepository;

    public SubmitFeedbackCommandHandler(IRepository<Feedback> feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<bool> HandleAsync(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = new Feedback
        {
            Content = request.Request.Content,
            UserId = request.Request.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _feedbackRepository.AddAsync(feedback, cancellationToken);
        return true;
    }
}
