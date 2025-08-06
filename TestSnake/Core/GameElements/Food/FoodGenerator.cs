using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Food
{
    public class FoodGenerator : IFoodGenerator
    {
        private readonly Random _random = new();

        public Position GenerateFood(List<Position> occupiedPositions, int width, int height)
        {
            if (occupiedPositions.Count >= width * height)
                return new Position(-1, -1);

            var availablePositions = new List<Position>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pos = new Position(x, y);
                    if (!occupiedPositions.Contains(pos))
                        availablePositions.Add(pos);
                }
            }

            if (availablePositions.Count == 0)
                return new Position(0, 0);

            return availablePositions[_random.Next(availablePositions.Count)];
        }

        public Position GenerateFoodWithTailAllowed(List<Position> snakeWithoutTail, List<Position> obstacles, int width, int height)
        {
            var occupiedPositions = new List<Position>(snakeWithoutTail);
            occupiedPositions.AddRange(obstacles);
            return GenerateFood(occupiedPositions, width, height);
        }
    }
}