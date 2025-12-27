namespace SocialMedia.Infrastructure;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(SocialMediaDbContext context) : base(context)
    {
    }
}