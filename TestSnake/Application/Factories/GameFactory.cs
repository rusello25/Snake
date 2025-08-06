using Microsoft.Extensions.Logging;
using TestSnake.Application.Events;
using TestSnake.Application.Interfaces;
using TestSnake.Application.Services;
using TestSnake.Core.Configuration;
using TestSnake.Core.GameElements.Food;
using TestSnake.Core.GameElements.Obstacles;
using TestSnake.Core.GameElements.Snake;
using TestSnake.Core.Physics;
using TestSnake.Core.Validation;
using TestSnake.Domain.Services;
using TestSnake.Infrastructure.Console;
using TestSnake.Infrastructure.IO.Input;
using TestSnake.Infrastructure.Managers.Record;
using TestSnake.Presentation.Rendering;

namespace TestSnake.Application.Factories
{
    /// <summary>
    /// Enumeration of game difficulty levels.
    /// </summary>
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    /// <summary>
    /// Enterprise-grade factory implementing Abstract Factory pattern for game component creation.
    /// Provides centralized creation of game objects with proper dependency management.
    /// </summary>
    public sealed class GameFactory(
        GameConfig config,
        IRecordManager recordManager,
        IObstacleManager obstacleManager,
        IFoodGenerator foodGenerator,
        ICollisionDetector collisionDetector,
        IEventAggregator eventAggregator,
        IGameRenderer renderer,
        IInputHandler inputHandler,
        IConsoleService consoleService,
        IScoreService scoreService,
        ILevelService levelService,
        IGameProgressionService gameProgressionService,
        ILogger<GameFactory> logger) : IGameFactory
    {
        private readonly GameConfig _config = Guard.NotNull(config);
        private readonly IRecordManager _recordManager = Guard.NotNull(recordManager);
        private readonly IObstacleManager _obstacleManager = Guard.NotNull(obstacleManager);
        private readonly IFoodGenerator _foodGenerator = Guard.NotNull(foodGenerator);
        private readonly ICollisionDetector _collisionDetector = Guard.NotNull(collisionDetector);
        private readonly IEventAggregator _eventAggregator = Guard.NotNull(eventAggregator);
        private readonly IGameRenderer _renderer = Guard.NotNull(renderer);
        private readonly IInputHandler _inputHandler = Guard.NotNull(inputHandler);
        private readonly IConsoleService _consoleService = Guard.NotNull(consoleService);
        private readonly IScoreService _scoreService = Guard.NotNull(scoreService);
        private readonly ILevelService _levelService = Guard.NotNull(levelService);
        private readonly IGameProgressionService _gameProgressionService = Guard.NotNull(gameProgressionService);
        private readonly ILogger<GameFactory> _logger = Guard.NotNull(logger);

        /// <inheritdoc />
        public GameLogic CreateGame()
        {
            try
            {
                _logger.LogInformation("Creating new game instance with configuration: {Width}x{Height}",
                    _config.Width, _config.Height);

                Guard.ValidGameConfiguration(_config.Width, _config.Height);

                SnakeShape snakeShape = CreateSnakeShape();

                var gameLogic = new GameLogic(
                    _config.Width,
                    _config.Height,
                    _scoreService,
                    _levelService,
                    snakeShape,
                    _foodGenerator,
                    _collisionDetector,
                    _obstacleManager,
                    _eventAggregator,
                    _gameProgressionService);

                _logger.LogDebug("Game instance created successfully");
                return gameLogic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create game instance");
                throw;
            }
        }

        /// <inheritdoc />
        public IGameRunner CreateGameRunner(GameLogic game)
        {
            try
            {
                Guard.NotNull(game);

                _logger.LogInformation("Creating game runner for game instance");

                var gameRunner = new GameRunner(
                    game,
                    _renderer,
                    _inputHandler,
                    _config,
                    _recordManager,
                    _consoleService);

                _logger.LogDebug("Game runner created successfully");
                return gameRunner;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create game runner");
                throw;
            }
        }

        /// <inheritdoc />
        public GameLogic CreateGameForDifficulty(GameDifficulty difficulty)
        {
            _logger.LogInformation("Creating game for difficulty: {Difficulty}", difficulty);

            // For now, return the standard game - can be enhanced later
            var game = CreateGame();

            _logger.LogDebug("Game created for difficulty: {Difficulty}", difficulty);
            return game;
        }

        /// <summary>
        /// Creates a new snake shape instance with proper initialization.
        /// </summary>
        /// <returns>Configured snake shape instance</returns>
        private SnakeShape CreateSnakeShape()
        {
            var snake = new SnakeShape();
            _logger.LogDebug("Snake shape created with default configuration");
            return snake;
        }
    }
}