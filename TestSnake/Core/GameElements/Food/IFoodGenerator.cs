using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Food
{
    public interface IFoodGenerator
    {
        Position GenerateFood(List<Position> occupiedPositions, int width, int height);
        Position GenerateFoodWithTailAllowed(List<Position> snakeWithoutTail, List<Position> obstacles, int width, int height);
    }
}