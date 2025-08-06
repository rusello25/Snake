using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.Specifications
{
    /// <summary>
    /// Base interface for specifications implementing the Specification pattern.
    /// </summary>
    /// <typeparam name="T">Type to apply specification to</typeparam>
    public interface ISpecification<in T>
    {
        /// <summary>
        /// Determines if the specification is satisfied by the given entity.
        /// </summary>
        /// <param name="entity">Entity to test</param>
        /// <returns>True if specification is satisfied, false otherwise</returns>
        bool IsSatisfiedBy(T entity);
    }

    /// <summary>
    /// Specification for determining if a position is within game boundaries.
    /// </summary>
    public sealed class WithinBoundsSpecification(int width, int height) : ISpecification<Position>
    {
        private readonly int _width = width;
        private readonly int _height = height;

        public bool IsSatisfiedBy(Position position)
        {
            return position.X >= 0 && position.X < _width && 
                   position.Y >= 0 && position.Y < _height;
        }
    }

    /// <summary>
    /// Specification for determining if a position is occupied by an obstacle.
    /// </summary>
    public sealed class ObstaclePositionSpecification(IReadOnlyList<Position> obstacles) : ISpecification<Position>
    {
        private readonly IReadOnlyList<Position> _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));

        public bool IsSatisfiedBy(Position position)
        {
            return _obstacles.Contains(position);
        }
    }

    /// <summary>
    /// Specification for determining if a position is occupied by the snake.
    /// </summary>
    public sealed class SnakePositionSpecification(IReadOnlyList<Position> snakeSegments) : ISpecification<Position>
    {
        private readonly IReadOnlyList<Position> _snakeSegments = snakeSegments ?? throw new ArgumentNullException(nameof(snakeSegments));

        public bool IsSatisfiedBy(Position position)
        {
            return _snakeSegments.Contains(position);
        }
    }

    /// <summary>
    /// Specification for determining if a position is valid for placing food.
    /// </summary>
    public sealed class ValidFoodPositionSpecification(
        int width, 
        int height, 
        IReadOnlyList<Position> obstacles, 
        IReadOnlyList<Position> snakeSegments) : ISpecification<Position>
    {
        private readonly WithinBoundsSpecification _boundsSpec = new(width, height);
        private readonly ObstaclePositionSpecification _obstacleSpec = new(obstacles);
        private readonly SnakePositionSpecification _snakeSpec = new(snakeSegments);

        public bool IsSatisfiedBy(Position position)
        {
            return _boundsSpec.IsSatisfiedBy(position) && 
                   !_obstacleSpec.IsSatisfiedBy(position) && 
                   !_snakeSpec.IsSatisfiedBy(position);
        }
    }

    /// <summary>
    /// Specification for determining if a move direction is valid.
    /// </summary>
    public sealed class ValidMoveDirectionSpecification : ISpecification<(int newDx, int newDy, int currentDx, int currentDy)>
    {
        public bool IsSatisfiedBy((int newDx, int newDy, int currentDx, int currentDy) moveData)
        {
            var (newDx, newDy, currentDx, currentDy) = moveData;
            
            // Prevent reverse direction (suicide move)
            return (currentDx != -newDx || currentDx == 0) && (currentDy != -newDy || currentDy == 0);
        }
    }
}