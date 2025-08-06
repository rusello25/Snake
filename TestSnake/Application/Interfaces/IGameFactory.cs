using TestSnake.Application.Factories;

namespace TestSnake.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for game component factories.
    /// Implements the Abstract Factory pattern for creating related game objects.
    /// </summary>
    public interface IGameFactory
    {
        /// <summary>
        /// Creates a new game logic instance with default configuration.
        /// </summary>
        /// <returns>Configured game logic instance</returns>
        GameLogic CreateGame();

        /// <summary>
        /// Creates a game runner for the specified game logic instance.
        /// </summary>
        /// <param name="game">Game logic instance to create runner for</param>
        /// <returns>Configured game runner instance</returns>
        IGameRunner CreateGameRunner(GameLogic game);

        /// <summary>
        /// Creates a game instance configured for the specified difficulty level.
        /// </summary>
        /// <param name="difficulty">Desired game difficulty</param>
        /// <returns>Game logic instance configured for the specified difficulty</returns>
        GameLogic CreateGameForDifficulty(GameDifficulty difficulty);
    }
}