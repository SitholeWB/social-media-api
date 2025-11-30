namespace SocialMedia.Application;

public class GetReportedUsersQueryHandler : IQueryHandler<GetReportedUsersQuery, List<ReportedUserDto>>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public GetReportedUsersQueryHandler(IReportRepository reportRepository, IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ReportedUserDto>> Handle(GetReportedUsersQuery request, CancellationToken cancellationToken)
    {
        // Get reports for posts
        var postReports = await _reportRepository.GetPostReportCountsByAuthorAsync();

        // Get reports for comments
        var commentReports = await _reportRepository.GetCommentReportCountsByAuthorAsync();

        // Combine
        var allReports = postReports
            .Concat(commentReports.Select(kvp => kvp))
            .GroupBy(x => x.Key)
            .Select(g => new { UserId = g.Key, TotalCount = g.Sum(x => x.Value) })
            .Where(x => x.TotalCount >= request.MinReports)
            .ToList();

        var userIds = allReports.Select(x => x.UserId).ToList();
        var users = new Dictionary<Guid, string>();

        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                users[userId] = user.Username;
            }
        }

        return allReports.Select(x => new ReportedUserDto(
            x.UserId,
            users.ContainsKey(x.UserId) ? users[x.UserId] : "Unknown",
            x.TotalCount
        )).OrderByDescending(x => x.ReportCount).ToList();
    }
}
