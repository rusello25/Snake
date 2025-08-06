using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.Strategies
{
    /// <summary>
    /// Strategy interface for food generation algorithms.
    /// </summary>
    public interface IFoodGenerationStrategy
    {
        /// <summary>
        /// Generates a food position using this strategy.
        /// </summary>
        /// <param name="occupiedPositions">Positions that are already occupied</param>
        /// <param name="width">Width of the game field</param>
        /// <param name="height">Height of the game field</param>
        /// <returns>Generated food position</returns>
        Position GenerateFood(IReadOnlyList<Position> occupiedPositions, int width, int height);

        /// <summary>
        /// Gets the name of this strategy.
        /// </summary>
        string StrategyName { get; }
    }

    /// <summary>
    /// Simple random food generation strategy.
    /// </summary>
    public sealed class RandomFoodGenerationStrategy : IFoodGenerationStrategy
    {
        private readonly Random _random = new();

        /// <inheritdoc />
        public Position GenerateFood(IReadOnlyList<Position> occupiedPositions, int width, int height)
        {
            var availablePositions = GetAvailablePositions(occupiedPositions, width, height);
            
            if (availablePositions.Count == 0)
                throw new InvalidOperationException("No available positions for food generation");

            var randomIndex = _random.Next(availablePositions.Count);
            return availablePositions[randomIndex];
        }

        /// <inheritdoc />
        public string StrategyName => "Random";

        private static List<Position> GetAvailablePositions(IReadOnlyList<Position> occupiedPositions, int width, int height)
        {
            var available = new List<Position>();
            var occupiedSet = new HashSet<Position>(occupiedPositions);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var position = new Position(x, y);
                    if (!occupiedSet.Contains(position))
                    {
                        available.Add(position);
                    }
                }
            }

            return available;
        }
    }

    /// <summary>
    /// Smart food generation strategy that places food away from the snake head.
    /// </summary>
    public sealed class SmartFoodGenerationStrategy : IFoodGenerationStrategy
    {
        private readonly Random _random = new();
        private const int PreferredDistance = 3;

        /// <inheritdoc />
        public Position GenerateFood(IReadOnlyList<Position> occupiedPositions, int width, int height)
        {
            if (occupiedPositions.Count == 0)
                return GenerateRandomPosition(width, height);

            var snakeHead = occupiedPositions[0]; // Assume first position is the head
            var availablePositions = GetAvailablePositions(occupiedPositions, width, height);
            
            if (availablePositions.Count == 0)
                throw new InvalidOperationException("No available positions for food generation");

            // Try to find positions away from the snake head
            var preferredPositions = availablePositions
                .Where(pos => CalculateDistance(pos, snakeHead) >= PreferredDistance)
                .ToList();

            var positionsToChooseFrom = preferredPositions.Count > 0 ? preferredPositions : availablePositions;
            var randomIndex = _random.Next(positionsToChooseFrom.Count);
            
            return positionsToChooseFrom[randomIndex];
        }

        /// <inheritdoc />
        public string StrategyName => "Smart";

        private Position GenerateRandomPosition(int width, int height)
        {
            return new Position(_random.Next(width), _random.Next(height));
        }

        private static List<Position> GetAvailablePositions(IReadOnlyList<Position> occupiedPositions, int width, int height)
        {
            var available = new List<Position>();
            var occupiedSet = new HashSet<Position>(occupiedPositions);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var position = new Position(x, y);
                    if (!occupiedSet.Contains(position))
                    {
                        available.Add(position);
                    }
                }
            }

            return available;
        }

        private static double CalculateDistance(Position pos1, Position pos2)
        {
            var dx = pos1.X - pos2.X;
            var dy = pos1.Y - pos2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    /// <summary>
    /// Context class that uses a food generation strategy.
    /// </summary>
    public sealed class FoodGenerationContext(IFoodGenerationStrategy strategy)
    {
        private IFoodGenerationStrategy _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

        /// <summary>
        /// Sets the food generation strategy.
        /// </summary>
        /// <param name="strategy">Strategy to use</param>
        public void SetStrategy(IFoodGenerationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        /// <summary>
        /// Generates food using the current strategy.
        /// </summary>
        /// <param name="occupiedPositions">Positions that are already occupied</param>
        /// <param name="width">Width of the game field</param>
        /// <param name="height">Height of the game field</param>
        /// <returns>Generated food position</returns>
        public Position GenerateFood(IReadOnlyList<Position> occupiedPositions, int width, int height)
        {
            return _strategy.GenerateFood(occupiedPositions, width, height);
        }

        /// <summary>
        /// Gets the current strategy name.
        /// </summary>
        public string CurrentStrategyName => _strategy.StrategyName;
    }
}