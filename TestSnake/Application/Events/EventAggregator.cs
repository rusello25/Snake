using TestSnake.Domain.Events;

namespace TestSnake.Application.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = [];

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var handlers))
            {
                handlers = [];
                _subscribers[type] = handlers;
            }
            handlers.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var handlers))
                handlers.Remove(handler);
        }

        public void Publish<T>(T eventData) where T : IGameEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(eventData);
                }
            }
        }
    }
}