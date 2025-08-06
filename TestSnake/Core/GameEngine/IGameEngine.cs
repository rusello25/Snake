namespace TestSnake.Core.GameEngine
{
    public interface IGameEngine : IDisposable
    {
        Task RunAsync(CancellationToken cancellationToken = default);
        void Stop();
        bool IsRunning { get; }
    }
}