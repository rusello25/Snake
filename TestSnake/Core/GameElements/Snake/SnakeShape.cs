using TestSnake.Domain.ValueObjects;

namespace TestSnake.Core.GameElements.Snake
{
    public class SnakeShape : ISnakeShape
    {
        private List<Position> _segments = [];

        public List<Position> Segments => [.. _segments];
        public Position Head => _segments.Count > 0 ? _segments[0] : new Position(0, 0);
        public Position Tail => _segments.Count > 0 ? _segments[^1] : new Position(0, 0);
        public int Length => _segments.Count;

        public void Initialize(Position startHead, int initialLength)
        {
            _segments = [];
            for (int i = 0; i < initialLength; i++)
            {
                _segments.Add(new Position(startHead.X - i, startHead.Y));
            }
        }

        public void Move(Position newHead)
        {
            _segments.Insert(0, newHead);
            _segments.RemoveAt(_segments.Count - 1);
        }

        public void Grow(Position newHead)
        {
            _segments.Insert(0, newHead);
        }

        public bool Contains(Position position)
        {
            return _segments.Contains(position);
        }

        public bool IsHeadAt(Position position)
        {
            return Head.Equals(position);
        }
    }
}