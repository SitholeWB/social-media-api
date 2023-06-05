namespace SocialMediaApi.Domain.Events
{
    public abstract class EventBase
    {
        protected EventBase()
        {
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset TimeStamp { get; private set; }
    }
}