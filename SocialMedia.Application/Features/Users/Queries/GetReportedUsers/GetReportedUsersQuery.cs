namespace SocialMedia.Application;

public record GetReportedUsersQuery(int MinReports) : IQuery<List<ReportedUserDto>>;
