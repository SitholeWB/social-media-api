using SocialMediaApi.Domain.Events;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.EventHandlers
{
    public class EventHandlerContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<Type, List<Type>> _mappings = new Dictionary<Type, List<Type>>();

        public EventHandlerContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void Subscribe<T, THandler>()
            where T : EventBase
            where THandler : IEventHandler<T>
        {
            var name = typeof(T);

            if (!_mappings.ContainsKey(name))
            {
                _mappings.Add(name, new List<Type> { });
            }

            _mappings[name].Add(typeof(THandler));
        }

        public static void UnSubscribe<T, THandler>()
            where T : EventBase
            where THandler : IEventHandler<T>
        {
            var name = typeof(T);
            _mappings[name].Remove(typeof(THandler));

            if (_mappings[name].Count == 0)
            {
                _mappings.Remove(name);
            }
        }

        public async Task PublishAsync<T>(T o) where T : EventBase
        {
            var name = typeof(T);

            if (_mappings.ContainsKey(name))
            {
                foreach (var handler in _mappings[name])
                {
                    var service = _serviceProvider.GetService(handler) as IEventHandler<T>;
                    await service!.RunAsync(o);
                }
            }
        }

        public Task PublishAndForgetAsync<T>(T o) where T : EventBase
        {
            var name = typeof(T);

            if (_mappings.ContainsKey(name))
            {
                foreach (var handler in _mappings[name])
                {
                    var service = _serviceProvider.GetService(handler) as IEventHandler<T>;
                    RunAndForget(service!, o);
                }
            }
            return Task.CompletedTask;
        }

        private static async void RunAndForget<T>(IEventHandler<T> handler, T o) where T : EventBase
        {
            await handler.RunAsync(o);
        }
    }
}