namespace TestSnake.Domain.ValueObjects
{
    /// <summary>
    /// Represents a direction vector as an immutable value object.
    /// </summary>
    public readonly struct Direction(int deltaX, int deltaY) : IEquatable<Direction>
    {
        public int DeltaX { get; } = deltaX;
        public int DeltaY { get; } = deltaY;

        public static readonly Direction Up = new(0, -1);
        public static readonly Direction Down = new(0, 1);
        public static readonly Direction Left = new(-1, 0);
        public static readonly Direction Right = new(1, 0);
        public static readonly Direction None = new(0, 0);

        public bool IsOpposite(Direction other)
        {
            return DeltaX == -other.DeltaX && DeltaY == -other.DeltaY;
        }

        public bool IsValid => DeltaX != 0 || DeltaY != 0;

        public override readonly bool Equals(object? obj)
        {
            return obj is Direction direction && Equals(direction);
        }

        public readonly bool Equals(Direction other)
        {
            return DeltaX == other.DeltaX && DeltaY == other.DeltaY;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(DeltaX, DeltaY);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }

        public override string ToString() => $"Direction({DeltaX}, {DeltaY})";
    }
}