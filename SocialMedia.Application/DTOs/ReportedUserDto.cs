
namespace SocialMedia.Application;

public record ReportedUserDto(Guid UserId, string Username, int ReportCount);
