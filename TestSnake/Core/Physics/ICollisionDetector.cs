using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.Physics
{
    public interface ICollisionDetector
    {
        bool IsOutOfBounds(Position position, int width, int height);
        bool IsOnSnake(Position position, List<Position> snake);
        bool IsSelfCollision(Position position, List<Position> snake);
        bool IsOnObstacle(Position position, List<Position> obstacles);
        bool IsValidPosition(Position position, int width, int height);
    }
}