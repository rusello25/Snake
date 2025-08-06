namespace TestSnake.Application.Services
{
    /// <summary>
    /// Defines the contract for level management services.
    /// </summary>
    public interface ILevelService
    {
        /// <summary>
        /// Resets the level to the initial value.
        /// </summary>
        void Reset();

        /// <summary>
        /// Updates the current level based on the provided score.
        /// </summary>
        /// <param name="score">Current score to calculate level from</param>
        void UpdateLevel(int score);

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <returns>The current level value</returns>
        int GetCurrentLevel();

        /// <summary>
        /// Gets the points required to reach the next level.
        /// </summary>
        /// <returns>Points needed for next level</returns>
        int GetPointsToNextLevel(int currentScore);

        /// <summary>
        /// Gets the speed multiplier for the current level.
        /// </summary>
        /// <returns>Speed multiplier based on current level</returns>
        double GetSpeedMultiplier();
    }
}