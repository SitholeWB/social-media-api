using SocialMediaApi.Domain.Events;

namespace SocialMediaApi.Interfaces
{
    public interface IEventHandler<in T> where T : EventBase
    {
        void Run(T obj);

        Task RunAsync(T obj);
    }
}