using TestSnake.Application.Interfaces;
using TestSnake.Core.Configuration;
using TestSnake.Core.Constants;
using TestSnake.Domain.ValueObjects;
using TestSnake.Infrastructure.Console;
using TestSnake.Infrastructure.IO.Input;
using TestSnake.Infrastructure.Managers.Record;
using TestSnake.Presentation.Rendering;

namespace TestSnake.Application
{
    public class GameRunner(
        GameLogic game, 
        IGameRenderer renderer, 
        IInputHandler inputHandler, 
        GameConfig config,
        IRecordManager recordManager,
        IConsoleService consoleService) : IGameRunner
    {
        private readonly GameLogic _game = game;
        private readonly IGameRenderer _renderer = renderer;
        private readonly IInputHandler _inputHandler = inputHandler;
        private readonly GameConfig _config = config;
        private readonly IRecordManager _recordManager = recordManager;
        private readonly IConsoleService _consoleService = consoleService;

        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            _renderer.ClearScreen();
            InitializeGameObstacles();

            while (!_game.IsGameOver() && !cancellationToken.IsCancellationRequested)
            {
                await ProcessInputAsync();
                _game.Update();
                UpdateObstaclesIfNeeded();
                await RenderGameAsync();
                await Task.Delay(GameConstants.GameUpdateIntervalMs, cancellationToken);
            }

            _game.EndGame();
            return _game.GetScore();
        }

        private void InitializeGameObstacles()
        {
            _game.ClearObstacles();
            _game.AddObstacle(new Position(5, 3));
        }

        private async Task ProcessInputAsync()
        {
            var key = _inputHandler.GetNextCommand();
            if (key.HasValue)
            {
                switch (key.Value)
                {
                    case ConsoleKey.UpArrow:    _game.ChangeDirection(0, -1); break;
                    case ConsoleKey.DownArrow:  _game.ChangeDirection(0, 1); break;
                    case ConsoleKey.LeftArrow:  _game.ChangeDirection(-1, 0); break;
                    case ConsoleKey.RightArrow: _game.ChangeDirection(1, 0); break;
                    case ConsoleKey.Escape:     return;
                }
            }
            await Task.CompletedTask;
        }

        private void UpdateObstaclesIfNeeded()
        {
            int level = _game.GetLevel();
            var obstacles = _game.GetObstacles();
            
            if (level > obstacles.Count && obstacles.Count < _config.MaxCactus)
            {
                _game.TryAddRandomObstacle();
            }
        }

        private async Task RenderGameAsync()
        {
            _consoleService.SetCursorPosition(0, 0);
            _renderer.DrawGame(
                _config.Width,
                _config.Height,
                _game.GetSnake(),
                _game.GetFood(),
                _game.GetObstacles(),
                _game.GetScore(),
                _game.GetLevel(),
                _recordManager.LoadRecord()
            );
            await Task.CompletedTask;
        }
    }
}