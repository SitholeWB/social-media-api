namespace SocialMedia.Application;

public class ReportPostCommandHandler : ICommandHandler<ReportPostCommand, Guid>
{
    private readonly IReportRepository _reportRepository;

    public ReportPostCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Guid> HandleAsync(ReportPostCommand command, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            PostId = command.PostId,
            ReporterId = command.ReporterId,
            Reason = command.Reason,
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _reportRepository.AddAsync(report, cancellationToken);
        return report.Id;
    }
}