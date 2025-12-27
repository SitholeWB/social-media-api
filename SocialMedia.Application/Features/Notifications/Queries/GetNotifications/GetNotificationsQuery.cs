namespace SocialMedia.Application;

public record GetNotificationsQuery(Guid UserId) : IQuery<List<NotificationDto>>;