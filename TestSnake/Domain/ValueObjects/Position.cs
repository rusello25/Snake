namespace TestSnake.Domain.ValueObjects
{
    /// <summary>
    /// Represents a position in the game field as an immutable value object.
    /// </summary>
    public readonly struct Position(int x, int y) : IEquatable<Position>
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        public override readonly bool Equals(object? obj)
        {
            return obj is Position position && Equals(position);
        }

        public readonly bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public static Position operator +(Position position, (int x, int y) vector)
        {
            return new Position(position.X + vector.x, position.Y + vector.y);
        }

        public static implicit operator (int x, int y)(Position position)
        {
            return (position.X, position.Y);
        }

        public static implicit operator Position((int x, int y) tuple)
        {
            return new Position(tuple.x, tuple.y);
        }

        public override string ToString() => $"({X}, {Y})";
    }
}