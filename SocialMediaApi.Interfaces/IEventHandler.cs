using SocialMediaApi.Domain.Events;

namespace SocialMediaApi.Interfaces
{
    public interface IEventHandler<in T> where T : EventBase
    {
        Task RunAsync(T obj);
    }
}