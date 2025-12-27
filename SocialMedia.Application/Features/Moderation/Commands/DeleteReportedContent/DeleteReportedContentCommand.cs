namespace SocialMedia.Application;

public record DeleteReportedContentCommand(int MinReports) : ICommand<int>;