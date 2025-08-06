using TestSnake.Domain.Events;

namespace TestSnake.Application.Events
{
    public interface IEventAggregator
    {
        void Subscribe<T>(Action<T> handler) where T : IGameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IGameEvent;
        void Publish<T>(T eventData) where T : IGameEvent;
    }
}