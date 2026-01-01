using System.Security.Claims;

namespace SocialMedia.API;

public static class RequestExtensions
{
    public static Guid? GetUserId(this ControllerBase request)
    {
        var userId = request?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
    }

    public static string? GetUserNames(this ControllerBase request)
    {
        return request?.User?.FindFirstValue(ClaimTypes.Name);
    }
}