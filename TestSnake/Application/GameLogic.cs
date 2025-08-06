using TestSnake.Application.Events;
using TestSnake.Application.Services;
using TestSnake.Core.Constants;
using TestSnake.Core.GameElements.Food;
using TestSnake.Core.GameElements.Obstacles;
using TestSnake.Core.GameElements.Snake;
using TestSnake.Core.Physics;
using TestSnake.Core.StateMachine;
using TestSnake.Core.Validation;
using TestSnake.Domain.Events;
using TestSnake.Domain.Services;
using TestSnake.Domain.ValueObjects;

namespace TestSnake.Application
{
    /// <summary>
    /// Core game logic orchestrator implementing the main game mechanics.
    /// Serves as the central coordinator for all game state and business rules.
    /// </summary>
    public sealed class GameLogic
    {
        private readonly int _width;
        private readonly int _height;
        private readonly ISnakeShape _snake;
        private readonly IFoodGenerator _foodGenerator;
        private readonly ICollisionDetector _collisionDetector;
        private readonly IObstacleManager _obstacleManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IScoreService _scoreService;
        private readonly ILevelService _levelService;
        private readonly IGameProgressionService _gameProgressionService;
        
        private int _dx;
        private int _dy;
        private Position _food;
        private IGameState _state = null!;

        public GameLogic(
            int width,
            int height,
            IScoreService scoreService,
            ILevelService levelService,
            ISnakeShape snake,
            IFoodGenerator foodGenerator,
            ICollisionDetector collisionDetector,
            IObstacleManager obstacleManager,
            IEventAggregator eventAggregator,
            IGameProgressionService gameProgressionService)
        {
            _width = Guard.Positive(width);
            _height = Guard.Positive(height);
            _scoreService = Guard.NotNull(scoreService);
            _levelService = Guard.NotNull(levelService);
            _snake = Guard.NotNull(snake);
            _foodGenerator = Guard.NotNull(foodGenerator);
            _collisionDetector = Guard.NotNull(collisionDetector);
            _obstacleManager = Guard.NotNull(obstacleManager);
            _eventAggregator = Guard.NotNull(eventAggregator);
            _gameProgressionService = Guard.NotNull(gameProgressionService);

            Guard.ValidGameConfiguration(_width, _height);
            InitializeGame();
        }

        /// <summary>
        /// Initializes the game to its starting state.
        /// </summary>
        public void InitializeGame()
        {
            _snake.Initialize(
                new Position(_width / 2, _height / 2), 
                GameConstants.DefaultInitialSnakeLength);
            
            _dx = 1;
            _dy = 0;
            _obstacleManager.ClearObstacles();
            _food = _foodGenerator.GenerateFood([.. _snake.Segments, .. _obstacleManager.GetObstacles()], _width, _height);
            _scoreService.Reset();
            _levelService.Reset();
            
            SetState(new RunningState(this));
            _eventAggregator.Publish(new GameStartedEvent(_width, _height));
        }

        /// <summary>
        /// Sets the current game state.
        /// </summary>
        /// <param name="newState">The new state to transition to</param>
        public void SetState(IGameState newState)
        {
            Guard.NotNull(newState);
            _state = newState;
        }

        /// <summary>
        /// Updates the game state for the current frame.
        /// </summary>
        public void Update()
        {
            _state?.Update();
        }

        /// <summary>
        /// Performs the core game update logic.
        /// </summary>
        public void UpdateGameCore()
        {
            var head = _snake.Head;
            var newHead = new Position(head.X + _dx, head.Y + _dy);

            if (IsCollision(newHead))
            {
                HandleGameOver(newHead);
                return;
            }

            if (IsFoodConsumption(newHead))
            {
                HandleFoodConsumption(newHead);
            }
            else
            {
                _snake.Move(newHead);
            }

            // Check if obstacles should be added based on progression service
            UpdateObstaclesIfNeeded();
        }

        /// <summary>
        /// Changes the snake's movement direction.
        /// </summary>
        /// <param name="newDx">New X direction</param>
        /// <param name="newDy">New Y direction</param>
        public void ChangeDirection(int newDx, int newDy)
        {
            // Prevent reverse direction (suicide move)
            if ((_dx != -newDx || _dx == 0) && (_dy != -newDy || _dy == 0))
            {
                _dx = newDx;
                _dy = newDy;
                _eventAggregator.Publish(new DirectionChangedEvent(newDx, newDy));
            }
        }

        /// <summary>
        /// Handles collision detection and game over logic.
        /// </summary>
        /// <param name="newHead">The new head position</param>
        private void HandleGameOver(Position newHead)
        {
            string collisionType = DetermineCollisionType(newHead);
            _eventAggregator.Publish(new CollisionEvent(newHead, collisionType));
            _eventAggregator.Publish(new GameOverEvent());
            SetState(new GameOverState(this));
        }

        /// <summary>
        /// Handles food consumption logic.
        /// </summary>
        /// <param name="newHead">The new head position</param>
        private void HandleFoodConsumption(Position newHead)
        {
            _snake.Grow(newHead);
            _scoreService.AddPoint();
            _eventAggregator.Publish(new FoodEatenEvent(_food, _scoreService.GetCurrentScore().Value));
            
            // Generate new food
            _food = _foodGenerator.GenerateFoodWithTailAllowed(
                [.. _snake.Segments.Take(_snake.Length - 1)], 
                _obstacleManager.GetObstacles(), 
                _width, 
                _height);
            
            _levelService.UpdateLevel(_scoreService.GetCurrentScore().Value);
        }

        /// <summary>
        /// Updates obstacles based on game progression logic.
        /// </summary>
        private void UpdateObstaclesIfNeeded()
        {
            var currentLevel = _levelService.GetCurrentLevel();
            var currentObstacles = _obstacleManager.GetObstacles().Count;
            var maxObstacles = _gameProgressionService.CalculateOptimalObstacleCount(currentLevel, _width, _height);

            if (_gameProgressionService.ShouldAddObstacle(currentLevel, currentObstacles, maxObstacles))
            {
                TryAddRandomObstacle();
            }
        }

        /// <summary>
        /// Determines the type of collision that occurred.
        /// </summary>
        /// <param name="position">Position where collision occurred</param>
        /// <returns>Description of collision type</returns>
        private string DetermineCollisionType(Position position)
        {
            if (_collisionDetector.IsOutOfBounds(position, _width, _height))
                return "Wall";
            if (_collisionDetector.IsOnObstacle(position, _obstacleManager.GetObstacles()))
                return "Obstacle";
            if (_collisionDetector.IsSelfCollision(position, _snake.Segments))
                return "Self";
            return "Unknown";
        }

        /// <summary>
        /// Checks if the given position results in a collision.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if collision occurs, false otherwise</returns>
        private bool IsCollision(Position position)
        {
            return _collisionDetector.IsOutOfBounds(position, _width, _height) ||
                   _collisionDetector.IsOnObstacle(position, _obstacleManager.GetObstacles()) ||
                   _collisionDetector.IsSelfCollision(position, _snake.Segments);
        }

        /// <summary>
        /// Checks if the given position results in food consumption.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if food is consumed, false otherwise</returns>
        private bool IsFoodConsumption(Position position)
        {
            return position.Equals(_food);
        }

        /// <summary>
        /// Sets the obstacles on the game field.
        /// </summary>
        /// <param name="obstacles">List of obstacle positions</param>
        public void SetObstacles(List<Position> obstacles)
        {
            _obstacleManager.SetObstacles(obstacles ?? []);
        }

        /// <summary>
        /// Clears all obstacles from the game field.
        /// </summary>
        public void ClearObstacles()
        {
            _obstacleManager.ClearObstacles();
        }

        /// <summary>
        /// Adds an obstacle at the specified position.
        /// </summary>
        /// <param name="position">Position to add obstacle</param>
        public void AddObstacle(Position position)
        {
            _obstacleManager.AddObstacle(position);
            _eventAggregator.Publish(new ObstacleAddedEvent(position));
        }

        /// <summary>
        /// Attempts to add a random obstacle to the game field.
        /// </summary>
        /// <returns>True if obstacle was added, false otherwise</returns>
        public bool TryAddRandomObstacle()
        {
            var result = _obstacleManager.TryAddObstacle(
                GetHeadPosition(),
                GetSnake(),
                GetFood(),
                _width,
                _height);
            
            if (result)
            {
                SetObstacles(_obstacleManager.GetObstacles());
            }
            
            return result;
        }

        /// <summary>
        /// Finalizes the game and updates records.
        /// </summary>
        public void EndGame()
        {
            _scoreService.UpdateRecordIfNeeded();
        }

        // Public getters
        public bool IsSelfCollision(Position position) => _collisionDetector.IsSelfCollision(position, _snake.Segments);
        public bool IsOnSnake(Position position) => _snake.Contains(position);
        public bool IsOutOfBounds(Position position) => _collisionDetector.IsOutOfBounds(position, _width, _height);
        public Position GetHeadPosition() => _snake.Head;
        public List<Position> GetSnake() => _snake.Segments;
        public List<Position> GetObstacles() => _obstacleManager.GetObstacles();
        public Position GetFood() => _food;
        public int GetScore() => _scoreService.GetCurrentScore().Value;
        public int GetLevel() => _levelService.GetCurrentLevel();
        public int GetCurrentRecord() => _scoreService.GetRecordScore().Value;
        public bool IsGameOver() => _state is GameOverState;
        public int GetWidth() => _width;
        public int GetHeight() => _height;
        public (int, int) GetDirection() => (_dx, _dy);

        // Setters for testing purposes
        public void SetFood(Position position) => _food = position;
        public void SetSnake(List<Position> snake)
        {
            Guard.NotNullOrEmpty(snake);
            _snake.Initialize(snake[0], snake.Count);
            for (int i = 1; i < snake.Count; i++)
            {
                _snake.Grow(snake[i]);
            }
        }
    }
}