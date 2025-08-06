using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Obstacles
{
    public interface IObstacleManager
    {
        List<Position> GetObstacles();
        void SetObstacles(List<Position> obstacles);
        void AddObstacle(Position position);
        void ClearObstacles();
        bool TryAddObstacle(Position headPosition, List<Position> snake, Position food, int width, int height);
    }
}