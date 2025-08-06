namespace TestSnake.Domain.Events
{
    public interface IGameEvent
    {
        DateTime Timestamp { get; }
    }

    public abstract record GameEvent(DateTime Timestamp) : IGameEvent
    {
        protected GameEvent() : this(DateTime.UtcNow) { }
    }
}