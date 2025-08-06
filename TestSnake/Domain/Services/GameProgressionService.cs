using TestSnake.Core.Validation;
using TestSnake.Domain.ValueObjects;

namespace TestSnake.Domain.Services
{
    /// <summary>
    /// Domain service responsible for game progression logic and calculations.
    /// Encapsulates complex business rules related to game advancement and scoring.
    /// </summary>
    public interface IGameProgressionService
    {
        /// <summary>
        /// Calculates the level based on the current score.
        /// </summary>
        /// <param name="score">The current score</param>
        /// <param name="pointsPerLevel">Points required per level</param>
        /// <returns>The calculated level</returns>
        int CalculateLevel(Score score, int pointsPerLevel);

        /// <summary>
        /// Calculates the speed multiplier for the given level.
        /// </summary>
        /// <param name="level">The current level</param>
        /// <returns>Speed multiplier for the level</returns>
        double CalculateSpeedMultiplier(int level);

        /// <summary>
        /// Calculates points needed to reach the next level.
        /// </summary>
        /// <param name="currentScore">The current score</param>
        /// <param name="pointsPerLevel">Points required per level</param>
        /// <returns>Points needed for next level</returns>
        int CalculatePointsToNextLevel(Score currentScore, int pointsPerLevel);

        /// <summary>
        /// Determines if a new obstacle should be added based on current game state.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="currentObstacles">Number of current obstacles</param>
        /// <param name="maxObstacles">Maximum allowed obstacles</param>
        /// <returns>True if obstacle should be added, false otherwise</returns>
        bool ShouldAddObstacle(int level, int currentObstacles, int maxObstacles);

        /// <summary>
        /// Calculates the optimal number of obstacles for the given level and field size.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="fieldWidth">Width of the game field</param>
        /// <param name="fieldHeight">Height of the game field</param>
        /// <returns>Optimal number of obstacles</returns>
        int CalculateOptimalObstacleCount(int level, int fieldWidth, int fieldHeight);
    }

    /// <summary>
    /// Implementation of game progression service with enterprise-grade business logic.
    /// </summary>
    public sealed class GameProgressionService : IGameProgressionService
    {
        private const int InitialLevel = 1;
        private const double BaseSpeedMultiplier = 1.0;
        private const double SpeedIncreasePerLevel = 0.1;
        private const double ObstacleProgressionFactor = 0.5;
        private const int MaxSpeedLevel = 20; // Cap speed increases at level 20

        /// <inheritdoc />
        public int CalculateLevel(Score score, int pointsPerLevel)
        {
            Guard.Positive(pointsPerLevel);

            return InitialLevel + (score.Value / pointsPerLevel);
        }

        /// <inheritdoc />
        public double CalculateSpeedMultiplier(int level)
        {
            Guard.Positive(level);

            // Cap speed increases to prevent impossible gameplay
            var effectiveLevel = Math.Min(level, MaxSpeedLevel);
            return BaseSpeedMultiplier + ((effectiveLevel - 1) * SpeedIncreasePerLevel);
        }

        /// <inheritdoc />
        public int CalculatePointsToNextLevel(Score currentScore, int pointsPerLevel)
        {
            Guard.Positive(pointsPerLevel);

            var currentLevel = CalculateLevel(currentScore, pointsPerLevel);
            var nextLevelScore = currentLevel * pointsPerLevel;
            return Math.Max(0, nextLevelScore - currentScore.Value);
        }

        /// <inheritdoc />
        public bool ShouldAddObstacle(int level, int currentObstacles, int maxObstacles)
        {
            Guard.Positive(level);
            Guard.NotNegative(currentObstacles);
            Guard.Positive(maxObstacles);

            // Add obstacles progressively based on level
            var targetObstacles = CalculateTargetObstacleCount(level, maxObstacles);
            return currentObstacles < targetObstacles && currentObstacles < maxObstacles;
        }

        /// <inheritdoc />
        public int CalculateOptimalObstacleCount(int level, int fieldWidth, int fieldHeight)
        {
            Guard.Positive(level);
            Guard.Positive(fieldWidth);
            Guard.Positive(fieldHeight);

            var fieldSize = fieldWidth * fieldHeight;
            var baseObstacles = Math.Max(1, (int)(fieldSize * 0.02)); // 2% of field
            var levelMultiplier = 1.0 + ((level - 1) * ObstacleProgressionFactor);
            
            var optimalCount = (int)(baseObstacles * levelMultiplier);
            
            // Cap at 25% of field to maintain playability
            var maxAllowed = (int)(fieldSize * 0.25);
            return Math.Min(optimalCount, maxAllowed);
        }

        /// <summary>
        /// Calculates the target number of obstacles for a given level.
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="maxObstacles">Maximum allowed obstacles</param>
        /// <returns>Target obstacle count</returns>
        private static int CalculateTargetObstacleCount(int level, int maxObstacles)
        {
            // Progressive obstacle addition: start with few, increase gradually
            var targetRatio = Math.Min(1.0, (level - 1) * 0.1); // 10% increase per level, capped at 100%
            return (int)(maxObstacles * targetRatio);
        }
    }
}