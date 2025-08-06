using TestSnake.Application.Events;
using TestSnake.Domain.ValueObjects;

namespace TestSnake.Application.Commands
{
    /// <summary>
    /// Base interface for all game commands implementing the Command pattern.
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// Undoes the command (if supported).
        /// </summary>
        void Undo();

        /// <summary>
        /// Gets whether this command can be undone.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Gets the command name for logging and debugging.
        /// </summary>
        string CommandName { get; }
    }

    /// <summary>
    /// Base implementation for game commands.
    /// </summary>
    public abstract class GameCommandBase(IEventAggregator eventAggregator) : IGameCommand
    {
        protected readonly IEventAggregator EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

        /// <inheritdoc />
        public abstract void Execute();

        /// <inheritdoc />
        public virtual void Undo()
        {
            throw new NotSupportedException($"Command {CommandName} does not support undo operation.");
        }

        /// <inheritdoc />
        public virtual bool CanUndo => false;

        /// <inheritdoc />
        public abstract string CommandName { get; }
    }

    /// <summary>
    /// Command for changing the snake's direction.
    /// </summary>
    public sealed class ChangeDirectionCommand(
        GameLogic game, 
        int newDx, 
        int newDy, 
        IEventAggregator eventAggregator) : GameCommandBase(eventAggregator)
    {
        private readonly GameLogic _game = game ?? throw new ArgumentNullException(nameof(game));
        private readonly int _newDx = newDx;
        private readonly int _newDy = newDy;
        private readonly (int dx, int dy) _previousDirection = game.GetDirection();

        /// <inheritdoc />
        public override void Execute()
        {
            _game.ChangeDirection(_newDx, _newDy);
        }

        /// <inheritdoc />
        public override void Undo()
        {
            _game.ChangeDirection(_previousDirection.dx, _previousDirection.dy);
        }

        /// <inheritdoc />
        public override bool CanUndo => true;

        /// <inheritdoc />
        public override string CommandName => "ChangeDirection";
    }

    /// <summary>
    /// Command for adding an obstacle to the game field.
    /// </summary>
    public sealed class AddObstacleCommand(
        GameLogic game, 
        Position position, 
        IEventAggregator eventAggregator) : GameCommandBase(eventAggregator)
    {
        private readonly GameLogic _game = game ?? throw new ArgumentNullException(nameof(game));
        private readonly Position _position = position;

        /// <inheritdoc />
        public override void Execute()
        {
            _game.AddObstacle(_position);
        }

        /// <inheritdoc />
        public override string CommandName => "AddObstacle";
    }

    /// <summary>
    /// Command for initializing a new game.
    /// </summary>
    public sealed class InitializeGameCommand(
        GameLogic game, 
        IEventAggregator eventAggregator) : GameCommandBase(eventAggregator)
    {
        private readonly GameLogic _game = game ?? throw new ArgumentNullException(nameof(game));

        /// <inheritdoc />
        public override void Execute()
        {
            _game.InitializeGame();
        }

        /// <inheritdoc />
        public override string CommandName => "InitializeGame";
    }

    /// <summary>
    /// Command invoker that manages command execution and undo operations.
    /// </summary>
    public sealed class GameCommandInvoker(int maxHistorySize = 10)
    {
        private readonly Stack<IGameCommand> _commandHistory = [];
        private readonly int _maxHistorySize = maxHistorySize;

        /// <summary>
        /// Executes a command and adds it to the history if it supports undo.
        /// </summary>
        /// <param name="command">Command to execute</param>
        public void ExecuteCommand(IGameCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            command.Execute();

            if (command.CanUndo)
            {
                _commandHistory.Push(command);

                // Limit history size to prevent memory leaks
                while (_commandHistory.Count > _maxHistorySize)
                {
                    _commandHistory.TryPop(out _);
                }
            }
        }

        /// <summary>
        /// Undoes the last command if possible.
        /// </summary>
        /// <returns>True if a command was undone, false otherwise</returns>
        public bool UndoLastCommand()
        {
            if (_commandHistory.Count > 0 && _commandHistory.TryPop(out var command))
            {
                command.Undo();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the command history.
        /// </summary>
        public void ClearHistory()
        {
            _commandHistory.Clear();
        }

        /// <summary>
        /// Gets whether there are commands available to undo.
        /// </summary>
        public bool CanUndo => _commandHistory.Count > 0;
    }
}