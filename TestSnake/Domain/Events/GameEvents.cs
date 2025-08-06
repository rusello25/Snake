using TestSnake.Domain.ValueObjects;

namespace TestSnake.Domain.Events
{
    /// <summary>
    /// Event published when a new game session is started.
    /// </summary>
    /// <param name="Width">The width of the game field</param>
    /// <param name="Height">The height of the game field</param>
    public record GameStartedEvent(int Width, int Height) : GameEvent;
    
    /// <summary>
    /// Event published when the game ends.
    /// </summary>
    public record GameOverEvent() : GameEvent;
    
    /// <summary>
    /// Event published when food is consumed by the snake.
    /// </summary>
    /// <param name="FoodPosition">The position where the food was consumed</param>
    /// <param name="NewScore">The new score after consuming the food</param>
    public record FoodEatenEvent(Position FoodPosition, int NewScore) : GameEvent;
    
    /// <summary>
    /// Event published when the player advances to a new level.
    /// </summary>
    /// <param name="NewLevel">The new level that was reached</param>
    public record LevelUpEvent(int NewLevel) : GameEvent;
    
    /// <summary>
    /// Event published when the player's score changes.
    /// </summary>
    /// <param name="Score">The new score value</param>
    public record ScoreChangedEvent(int Score) : GameEvent;
    
    /// <summary>
    /// Event published when a new high score record is set.
    /// </summary>
    /// <param name="NewRecord">The new record score value</param>
    public record NewRecordSetEvent(int NewRecord) : GameEvent;
    
    /// <summary>
    /// Event published when the game over menu is displayed.
    /// </summary>
    /// <param name="Score">The final score of the game session</param>
    /// <param name="Record">The current high score record</param>
    public record GameOverMenuDisplayedEvent(int Score, int Record) : GameEvent;
    
    /// <summary>
    /// Event published when the snake's direction changes.
    /// </summary>
    /// <param name="DeltaX">The change in X direction</param>
    /// <param name="DeltaY">The change in Y direction</param>
    public record DirectionChangedEvent(int DeltaX, int DeltaY) : GameEvent;
    
    /// <summary>
    /// Event published when a new obstacle is added to the game field.
    /// </summary>
    /// <param name="ObstaclePosition">The position where the obstacle was added</param>
    public record ObstacleAddedEvent(Position ObstaclePosition) : GameEvent;

    /// <summary>
    /// Event published when the game is paused.
    /// </summary>
    public record GamePausedEvent() : GameEvent;

    /// <summary>
    /// Event published when the game is resumed from a paused state.
    /// </summary>
    public record GameResumedEvent() : GameEvent;

    /// <summary>
    /// Event published when the snake collides with an obstacle or itself.
    /// </summary>
    /// <param name="CollisionPosition">The position where the collision occurred</param>
    /// <param name="CollisionType">The type of collision that occurred</param>
    public record CollisionEvent(Position CollisionPosition, string CollisionType) : GameEvent;
}