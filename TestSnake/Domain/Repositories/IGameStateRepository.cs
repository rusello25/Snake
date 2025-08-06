using TestSnake.Domain.ValueObjects;

namespace TestSnake.Domain.Repositories
{
    /// <summary>
    /// Generic repository interface following Repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity identifier</typeparam>
    public interface IRepository<TEntity, in TId>
        where TEntity : class
        where TId : notnull
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity if found, null otherwise</returns>
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>All entities</returns>
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Added entity</returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists.
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Specialized repository interface for game state persistence.
    /// </summary>
    public interface IGameStateRepository
    {
        /// <summary>
        /// Saves the current game state.
        /// </summary>
        /// <param name="gameState">Game state to save</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the save operation</returns>
        Task SaveGameStateAsync(GameStateSnapshot gameState, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads a saved game state.
        /// </summary>
        /// <param name="saveSlotId">Save slot identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Loaded game state or null if not found</returns>
        Task<GameStateSnapshot?> LoadGameStateAsync(string saveSlotId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all available save slots.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of save slot identifiers</returns>
        Task<IReadOnlyList<string>> GetSaveSlotsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a saved game state.
        /// </summary>
        /// <param name="saveSlotId">Save slot identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteSaveStateAsync(string saveSlotId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a snapshot of the game state for persistence.
    /// </summary>
    public sealed record GameStateSnapshot
    {
        public required string SaveSlotId { get; init; }
        public required DateTime SavedAt { get; init; }
        public required Score CurrentScore { get; init; }
        public required int CurrentLevel { get; init; }
        public required IReadOnlyList<Position> SnakeSegments { get; init; }
        public required Position FoodPosition { get; init; }
        public required IReadOnlyList<Position> Obstacles { get; init; }
        public required (int X, int Y) Direction { get; init; }
        public required int GameWidth { get; init; }
        public required int GameHeight { get; init; }
        public string? PlayerName { get; init; }
        public TimeSpan PlayTime { get; init; }

        /// <summary>
        /// Creates a game state snapshot from basic game information.
        /// </summary>
        /// <param name="saveSlotId">Save slot identifier</param>
        /// <param name="score">Current score</param>
        /// <param name="level">Current level</param>
        /// <param name="snakeSegments">Snake segments</param>
        /// <param name="foodPosition">Food position</param>
        /// <param name="obstacles">Obstacle positions</param>
        /// <param name="direction">Movement direction</param>
        /// <param name="gameWidth">Game field width</param>
        /// <param name="gameHeight">Game field height</param>
        /// <param name="playerName">Optional player name</param>
        /// <param name="playTime">Current play time</param>
        /// <returns>Game state snapshot</returns>
        public static GameStateSnapshot Create(
            string saveSlotId,
            int score,
            int level,
            IReadOnlyList<Position> snakeSegments,
            Position foodPosition,
            IReadOnlyList<Position> obstacles,
            (int X, int Y) direction,
            int gameWidth,
            int gameHeight,
            string? playerName = null,
            TimeSpan playTime = default)
        {
            return new GameStateSnapshot
            {
                SaveSlotId = saveSlotId,
                SavedAt = DateTime.UtcNow,
                CurrentScore = new Score(score),
                CurrentLevel = level,
                SnakeSegments = snakeSegments,
                FoodPosition = foodPosition,
                Obstacles = obstacles,
                Direction = direction,
                GameWidth = gameWidth,
                GameHeight = gameHeight,
                PlayerName = playerName,
                PlayTime = playTime
            };
        }
    }
}