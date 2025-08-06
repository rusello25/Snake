using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.Physics
{
    public class CollisionDetector : ICollisionDetector
    {
        public bool IsOutOfBounds(Position position, int width, int height)
        {
            return position.X < 0 || position.Y < 0 || position.X >= width || position.Y >= height;
        }

        public bool IsOnSnake(Position position, List<Position> snake)
        {
            return snake.Contains(position);
        }

        public bool IsSelfCollision(Position position, List<Position> snake)
        {
            return snake.Skip(1).Contains(position);
        }

        public bool IsOnObstacle(Position position, List<Position> obstacles)
        {
            return obstacles.Contains(position);
        }

        public bool IsValidPosition(Position position, int width, int height)
        {
            return !IsOutOfBounds(position, width, height);
        }
    }
}