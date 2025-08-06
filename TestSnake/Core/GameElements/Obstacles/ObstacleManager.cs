using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Obstacles
{
    public class ObstacleManager : IObstacleManager
    {
        private List<Position> _obstacles = [];
        private readonly Random _random = new();
        
        public List<Position> GetObstacles() => [.. _obstacles];
        public void SetObstacles(List<Position> obstacles) => _obstacles = [.. obstacles];
        public void AddObstacle(Position obstacle) => _obstacles.Add(obstacle);
        public void ClearObstacles() => _obstacles.Clear();

        public bool TryAddObstacle(Position headPosition, List<Position> snake, Position food, int width, int height)
        {
            return TryAddObstacle(headPosition, snake, food, width, height, 3);
        }

        private bool TryAddObstacle(Position headPosition, List<Position> snake, Position food, int width, int height, int minDistance)
        {
            for (int attempts = 0; attempts < 50; attempts++)
            {
                var candidate = new Position(_random.Next(width), _random.Next(height));
                
                if (Math.Abs(candidate.X - headPosition.X) < minDistance && 
                    Math.Abs(candidate.Y - headPosition.Y) < minDistance)
                    continue;
                
                if (snake.Contains(candidate) || candidate.Equals(food) || _obstacles.Contains(candidate))
                    continue;
                
                _obstacles.Add(candidate);
                return true;
            }
            return false;
        }
    }
}