namespace TestSnake.Domain.ValueObjects
{
    /// <summary>
    /// Represents a game score as an immutable value object.
    /// </summary>
    public readonly struct Score(int value) : IEquatable<Score>, IComparable<Score>
    {
        public int Value { get; } = value >= 0 ? value : throw new ArgumentException("Score cannot be negative", nameof(value));

        public static readonly Score Zero = new(0);

        public Score Add(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points to add cannot be negative", nameof(points));
            
            return new Score(Value + points);
        }

        public Score AddOne() => Add(1);

        public bool IsNewRecord(Score previousRecord) => Value > previousRecord.Value;

        public override readonly bool Equals(object? obj)
        {
            return obj is Score score && Equals(score);
        }

        public readonly bool Equals(Score other)
        {
            return Value == other.Value;
        }

        public readonly int CompareTo(Score other)
        {
            return Value.CompareTo(other.Value);
        }

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Score left, Score right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Score left, Score right)
        {
            return !(left == right);
        }

        public static bool operator >(Score left, Score right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(Score left, Score right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(Score left, Score right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(Score left, Score right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static implicit operator int(Score score) => score.Value;
        public static implicit operator Score(int value) => new(value);

        public override string ToString() => Value.ToString();
    }
}