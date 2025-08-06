using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Snake
{
    public interface ISnakeShape
    {
        List<Position> Segments { get; }
        Position Head { get; }
        Position Tail { get; }
        int Length { get; }
        void Move(Position newHead);
        void Grow(Position newHead);
        bool Contains(Position position);
        bool IsHeadAt(Position position);
        void Initialize(Position startHead, int initialLength);
    }
}