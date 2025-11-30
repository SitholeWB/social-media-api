
namespace SocialMedia.Application;

public record NotificationDto(Guid Id, string Message, NotificationType Type, Guid? RelatedId, bool IsRead, DateTime CreatedAt);
