namespace TestSnake.Application.Interfaces
{
    public interface IGameRunner
    {
        Task<int> RunAsync(CancellationToken cancellationToken = default);
    }
}