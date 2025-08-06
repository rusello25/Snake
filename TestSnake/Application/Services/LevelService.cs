using TestSnake.Application.Events;
using TestSnake.Domain.Events;

namespace TestSnake.Application.Services
{
    /// <summary>
    /// Service responsible for managing game levels and progression.
    /// Calculates level advancement based on score and provides level-related game mechanics.
    /// </summary>
    public sealed class LevelService : ILevelService
    {
        private readonly int _pointsPerLevel;
        private readonly IEventAggregator _eventAggregator;
        private int _currentLevel;

        private const int InitialLevel = 1;
        private const double BaseSpeedMultiplier = 1.0;
        private const double SpeedIncreasePerLevel = 0.1;

        /// <summary>
        /// Initializes a new instance of the LevelService class.
        /// </summary>
        /// <param name="pointsPerLevel">Points required to advance to next level</param>
        /// <param name="eventAggregator">Event aggregator for publishing level events</param>
        public LevelService(int pointsPerLevel, IEventAggregator eventAggregator)
        {
            if (pointsPerLevel <= 0)
                throw new ArgumentException("Points per level must be positive", nameof(pointsPerLevel));

            _pointsPerLevel = pointsPerLevel;
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _currentLevel = InitialLevel;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _currentLevel = InitialLevel;
        }

        /// <inheritdoc />
        public void UpdateLevel(int score)
        {
            if (score < 0)
                throw new ArgumentException("Score cannot be negative", nameof(score));

            var newLevel = CalculateLevelFromScore(score);
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;
                _eventAggregator.Publish(new LevelUpEvent(_currentLevel));
            }
        }

        /// <inheritdoc />
        public int GetCurrentLevel() => _currentLevel;

        /// <inheritdoc />
        public int GetPointsToNextLevel(int currentScore)
        {
            if (currentScore < 0)
                throw new ArgumentException("Current score cannot be negative", nameof(currentScore));

            var nextLevelScore = _currentLevel * _pointsPerLevel;
            return Math.Max(0, nextLevelScore - currentScore);
        }

        /// <inheritdoc />
        public double GetSpeedMultiplier()
        {
            return BaseSpeedMultiplier + ((_currentLevel - 1) * SpeedIncreasePerLevel);
        }

        /// <summary>
        /// Calculates the level based on the provided score.
        /// </summary>
        /// <param name="score">Score to calculate level from</param>
        /// <returns>The calculated level</returns>
        private int CalculateLevelFromScore(int score)
        {
            return InitialLevel + (score / _pointsPerLevel);
        }
    }
}