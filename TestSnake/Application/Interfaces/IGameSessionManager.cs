namespace TestSnake.Application.Interfaces
{
    public record GameSessionResult(bool ShouldExit, int FinalScore = 0);

    public interface IGameSessionManager
    {
        Task<GameSessionResult> RunSessionAsync(CancellationToken cancellationToken = default);
    }
}