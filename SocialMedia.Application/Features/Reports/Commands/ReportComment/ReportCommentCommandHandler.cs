namespace SocialMedia.Application;

public class ReportCommentCommandHandler : ICommandHandler<ReportCommentCommand, Guid>
{
    private readonly IReportRepository _reportRepository;

    public ReportCommentCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Guid> HandleAsync(ReportCommentCommand command, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            CommentId = command.CommentId,
            ReporterId = command.ReporterId,
            Reason = command.Reason,
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _reportRepository.AddAsync(report, cancellationToken);
        return report.Id;
    }
}