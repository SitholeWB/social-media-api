using SocialMediaApi.Domain.Events;

namespace SocialMediaApi.Interfaces
{
    public interface IEventHandler<T> where T : EventBase
    {
        void Run(T obj);

        Task RunAsync(T obj);
    }
}