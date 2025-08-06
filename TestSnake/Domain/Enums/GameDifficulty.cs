namespace TestSnake.Domain.Enums
{
    /// <summary>
    /// Represents different difficulty levels for the game.
    /// </summary>
    public enum GameDifficulty
    {
        Easy = 200,      // 200ms between moves
        Normal = 150,    // 150ms between moves  
        Hard = 100,      // 100ms between moves
        Expert = 50      // 50ms between moves
    }
}