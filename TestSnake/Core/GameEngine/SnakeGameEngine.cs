using TestSnake.Application.Events;
using TestSnake.Application.Interfaces;
using TestSnake.Core.Configuration;
using TestSnake.Infrastructure.Audio.Services;
using TestSnake.Infrastructure.Managers.Record;
using TestSnake.Presentation.Rendering;

namespace TestSnake.Core.GameEngine
{
    public class SnakeGameEngine(
        GameConfig config,
        IGameRenderer renderer,
        IRecordManager recordManager,
        ISoundManager soundManager,
        IEventAggregator eventAggregator,
        IGameSessionManager sessionManager) : IGameEngine
    {
        private readonly GameConfig _config = config;
        private readonly IGameRenderer _renderer = renderer;
        private readonly IRecordManager _recordManager = recordManager;
        private readonly ISoundManager _soundManager = soundManager;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly IGameSessionManager _sessionManager = sessionManager;
        private bool _isRunning;
        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsRunning => _isRunning;

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            _isRunning = true;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var sessionResult = await _sessionManager.RunSessionAsync(_cancellationTokenSource.Token);
                    
                    if (sessionResult.ShouldExit)
                        break;
                }
            }
            finally
            {
                _isRunning = false;
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}