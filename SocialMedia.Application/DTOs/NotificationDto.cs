namespace SocialMedia.Application;

public record NotificationDto(Guid Id, Guid TenantId, string Message, NotificationType Type, Guid? RelatedId, bool IsRead, DateTimeOffset CreatedAt);