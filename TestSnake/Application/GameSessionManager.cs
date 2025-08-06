using TestSnake.Application.Events;
using TestSnake.Application.Interfaces;
using TestSnake.Core.Configuration;
using TestSnake.Domain.Events;
using TestSnake.Infrastructure.Audio.Services;
using TestSnake.Infrastructure.Console;
using TestSnake.Infrastructure.Managers.Record;
using TestSnake.Presentation.Rendering;

namespace TestSnake.Application
{
    public class GameSessionManager(
        GameConfig config,
        IGameRenderer renderer,
        IRecordManager recordManager,
        IEventAggregator eventAggregator,
        IGameFactory gameFactory,
        ISoundManager soundManager,
        IConsoleService consoleService) : IGameSessionManager
    {
        private readonly GameConfig _config = config;
        private readonly IGameRenderer _renderer = renderer;
        private readonly IRecordManager _recordManager = recordManager;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly IGameFactory _gameFactory = gameFactory;
        private readonly ISoundManager _soundManager = soundManager;
        private readonly IConsoleService _consoleService = consoleService;

        public async Task<GameSessionResult> RunSessionAsync(CancellationToken cancellationToken = default)
        {
            _renderer.ClearScreen();

            var (shouldExit, soundEnabled) = await HandleIntroScreenAsync(cancellationToken);
            if (shouldExit)
                return new GameSessionResult(true);

            var (score, _) = await RunGameLoopAsync(soundEnabled, cancellationToken);
            
            var menuResult = await HandleGameOverMenuAsync(score, cancellationToken);
            
            return new GameSessionResult(menuResult, score);
        }

        private async Task<(bool ShouldExit, bool SoundEnabled)> HandleIntroScreenAsync(CancellationToken cancellationToken)
        {
            using var introCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var introTask = _soundManager.PlayIntroLoopAsync(introCts.Token);

            _renderer.ShowStartScreen(_recordManager.LoadRecord());
            var keyInfo = _consoleService.ReadKey(true);
            
            introCts.Cancel();
            await introTask;

            bool soundEnabled = keyInfo.Key != ConsoleKey.N;
            return (false, soundEnabled);
        }

        private async Task<(int Score, int Record)> RunGameLoopAsync(bool soundEnabled, CancellationToken cancellationToken)
        {
            _soundManager.SetSoundEnabled(soundEnabled);
            
            if (soundEnabled)
            {
                _ = _soundManager.PlayFanfareAsync(cancellationToken);
            }

            var game = _gameFactory.CreateGame();
            var gameRunner = _gameFactory.CreateGameRunner(game);
            
            var score = await gameRunner.RunAsync(cancellationToken);
            var _1 = _recordManager.LoadRecord(); // Discard pattern to satisfy IDE0059
            
            return (score, _1);
        }

        private async Task<bool> HandleGameOverMenuAsync(int score, CancellationToken cancellationToken)
        {
            var record = _recordManager.LoadRecord();
            
            _renderer.ClearScreen();
            _eventAggregator.Publish(new GameOverMenuDisplayedEvent(score, record));
            _renderer.ShowGameOverMenu(score, record);

            // Use Task.Run to make this truly async and allow cancellation
            return await Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var key = _consoleService.ReadKey(true).Key;
                    return key switch
                    {
                        ConsoleKey.R => false, // Restart
                        ConsoleKey.E => true,  // Exit
                        _ => false // Invalid key, restart
                    };
                }
                return true; // Exit if cancelled
            }, cancellationToken);
        }
    }
}