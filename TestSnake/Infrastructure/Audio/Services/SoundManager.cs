using TestSnake.Application.Events;
using TestSnake.Domain.Events;
using TestSnake.Infrastructure.Audio.Midi;

namespace TestSnake.Infrastructure.Audio.Services
{
    public class SoundManager : ISoundManager
    {
        private readonly IMidiPlayer _midiPlayer;
        private readonly IEventAggregator _eventAggregator;
        private IMidiPlayer _currentPlayer;
        private bool _soundEnabled = true;

        public bool IsSoundEnabled => _soundEnabled;

        public SoundManager(IMidiPlayer midiPlayer, IEventAggregator eventAggregator)
        {
            _midiPlayer = midiPlayer;
            _eventAggregator = eventAggregator;
            _currentPlayer = midiPlayer;
            
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _eventAggregator.Subscribe<GameOverMenuDisplayedEvent>(_ => PlayGameOverSound());
            _eventAggregator.Subscribe<FoodEatenEvent>(_ => PlayEatSound());
            _eventAggregator.Subscribe<LevelUpEvent>(_ => PlayLevelUpSound());
        }

        public void SetSoundEnabled(bool enabled)
        {
            _soundEnabled = enabled;
            _currentPlayer = enabled ? _midiPlayer : new SilentMidiPlayer();
        }

        public Task PlayIntroLoopAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => _currentPlayer.PlayIntroLoop(cancellationToken), cancellationToken);
        }

        public Task PlayFanfareAsync(CancellationToken cancellationToken = default)
        {
            return _currentPlayer.PlayFanfareAsync(cancellationToken);
        }

        public void PlayEatSound()
        {
            _currentPlayer.PlayEat();
        }

        public void PlayGameOverSound()
        {
            _currentPlayer.PlayGameOver();
        }

        public void PlayLevelUpSound()
        {
            _currentPlayer.PlayLevelUp();
        }

        public void StopAllSounds()
        {
            _currentPlayer.StopMelody();
            _currentPlayer.StopGameOverMelody();
        }

        public void Dispose()
        {
            StopAllSounds();
            if (_currentPlayer is SilentMidiPlayer silentPlayer)
            {
                silentPlayer.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}