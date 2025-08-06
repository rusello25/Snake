using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TestSnake.Domain.Exceptions;

namespace TestSnake.Core.Validation
{
    /// <summary>
    /// Provides guard clauses for parameter validation throughout the application.
    /// Implements the Guard pattern to ensure robust input validation and fail-fast behavior.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Ensures that the specified argument is not null.
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="argument">The argument to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated argument</returns>
        /// <exception cref="ArgumentNullException">Thrown when argument is null</exception>
        public static T NotNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
            where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified string argument is not null or empty.
        /// </summary>
        /// <param name="argument">The string argument to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated string argument</returns>
        /// <exception cref="ArgumentException">Thrown when argument is null or empty</exception>
        public static string NotNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Value cannot be null or empty.", parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified string argument is not null, empty, or whitespace.
        /// </summary>
        /// <param name="argument">The string argument to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated string argument</returns>
        /// <exception cref="ArgumentException">Thrown when argument is null, empty, or whitespace</exception>
        public static string NotNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException("Value cannot be null, empty, or whitespace.", parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified numeric argument is positive.
        /// </summary>
        /// <param name="argument">The numeric argument to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated argument</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when argument is not positive</exception>
        public static int Positive(int argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, argument, "Value must be positive.");
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified numeric argument is not negative.
        /// </summary>
        /// <param name="argument">The numeric argument to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated argument</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when argument is negative</exception>
        public static int NotNegative(int argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, argument, "Value cannot be negative.");
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified argument is within the specified range.
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="min">The minimum allowed value (inclusive)</param>
        /// <param name="max">The maximum allowed value (inclusive)</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated argument</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when argument is outside the specified range</exception>
        public static int InRange(int argument, int min, int max, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < min || argument > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, argument, $"Value must be between {min} and {max}.");
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified collection is not null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="argument">The collection to validate</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The validated collection</returns>
        /// <exception cref="ArgumentException">Thrown when collection is null or empty</exception>
        public static IEnumerable<T> NotNullOrEmpty<T>([NotNull] IEnumerable<T>? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (!argument.Any())
            {
                throw new ArgumentException("Collection cannot be empty.", parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures that the specified game configuration is valid.
        /// </summary>
        /// <param name="width">The game field width</param>
        /// <param name="height">The game field height</param>
        /// <exception cref="InvalidGameConfigurationException">Thrown when configuration is invalid</exception>
        public static void ValidGameConfiguration(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new InvalidGameConfigurationException($"Game dimensions must be positive. Width: {width}, Height: {height}");
            }

            if (width < 5 || height < 5)
            {
                throw new InvalidGameConfigurationException($"Game dimensions too small for gameplay. Minimum 5x5. Width: {width}, Height: {height}");
            }
        }

        /// <summary>
        /// Ensures that the specified condition is true.
        /// </summary>
        /// <param name="condition">The condition to validate</param>
        /// <param name="message">The error message if condition is false</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentException">Thrown when condition is false</exception>
        public static void That(bool condition, string message, [CallerArgumentExpression(nameof(condition))] string? parameterName = null)
        {
            if (!condition)
            {
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}