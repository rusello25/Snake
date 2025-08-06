using System.Runtime.Serialization;

namespace TestSnake.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all game-related exceptions.
    /// Provides common functionality and ensures consistent error handling.
    /// </summary>
    [Serializable]
    public abstract class GameException : Exception
    {
        /// <summary>
        /// Gets the error code associated with this exception.
        /// </summary>
        public abstract string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the GameException class.
        /// </summary>
        protected GameException() : base() { }

        /// <summary>
        /// Initializes a new instance of the GameException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        protected GameException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the GameException class with a specified error message and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        protected GameException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the GameException class with serialized data.
        /// This constructor is required for serialization and marked obsolete by .NET 8.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        protected GameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invalid game operation is attempted.
    /// </summary>
    [Serializable]
    public sealed class InvalidGameOperationException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_OP_001";

        public InvalidGameOperationException(string operation) 
            : base($"Invalid game operation: {operation}") { }

        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidGameOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when trying to perform an action on a finished game.
    /// </summary>
    [Serializable]
    public sealed class GameAlreadyFinishedException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_STATE_002";

        public GameAlreadyFinishedException() 
            : base("Cannot perform operation on a finished game") { }

        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private GameAlreadyFinishedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invalid position is used in the game.
    /// </summary>
    [Serializable]
    public sealed class InvalidPositionException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_POS_001";

        public InvalidPositionException(string message) : base(message) { }

        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidPositionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invalid direction change is attempted.
    /// </summary>
    [Serializable]
    public sealed class InvalidDirectionException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_DIR_001";

        public InvalidDirectionException(string message) : base(message) { }

        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidDirectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when an invalid game configuration is detected.
    /// </summary>
    [Serializable]
    public sealed class InvalidGameConfigurationException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_CONFIG_001";

        public InvalidGameConfigurationException() : base("Invalid game configuration detected.") { }
        public InvalidGameConfigurationException(string message) : base(message) { }
        public InvalidGameConfigurationException(string message, Exception innerException) : base(message, innerException) { }
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidGameConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when an invalid move operation is attempted.
    /// </summary>
    [Serializable]
    public sealed class InvalidMoveException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_MOVE_001";

        public InvalidMoveException() : base("Invalid move operation attempted.") { }
        public InvalidMoveException(string message) : base(message) { }
        public InvalidMoveException(string message, Exception innerException) : base(message, innerException) { }
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidMoveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when a game state operation is invalid for the current state.
    /// </summary>
    [Serializable]
    public sealed class InvalidGameStateException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "GAME_STATE_001";

        public InvalidGameStateException() : base("Invalid game state operation.") { }
        public InvalidGameStateException(string message) : base(message) { }
        public InvalidGameStateException(string message, Exception innerException) : base(message, innerException) { }
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private InvalidGameStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when audio system operations fail.
    /// </summary>
    [Serializable]
    public sealed class AudioSystemException : GameException
    {
        /// <inheritdoc />
        public override string ErrorCode => "AUDIO_001";

        public AudioSystemException() : base("Audio system operation failed.") { }
        public AudioSystemException(string message) : base(message) { }
        public AudioSystemException(string message, Exception innerException) : base(message, innerException) { }
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        private AudioSystemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}