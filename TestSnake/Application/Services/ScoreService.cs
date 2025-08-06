using TestSnake.Application.Events;
using TestSnake.Domain.Events;
using TestSnake.Domain.ValueObjects;
using TestSnake.Infrastructure.Managers.Record;

namespace TestSnake.Application.Services
{
    /// <summary>
    /// Service responsible for managing game score and record tracking.
    /// Implements domain logic for score calculation and record management.
    /// </summary>
    public sealed class ScoreService(IRecordManager recordManager, IEventAggregator eventAggregator) : IScoreService
    {
        private readonly IRecordManager _recordManager = recordManager ?? throw new ArgumentNullException(nameof(recordManager));
        private readonly IEventAggregator _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        private Score _currentScore = Score.Zero;

        /// <inheritdoc />
        public void Reset()
        {
            _currentScore = Score.Zero;
            _eventAggregator.Publish(new ScoreChangedEvent(_currentScore.Value));
        }

        /// <inheritdoc />
        public void AddPoint()
        {
            AddPoints(1);
        }

        /// <inheritdoc />
        public void AddPoints(int points)
        {
            if (points <= 0)
                throw new ArgumentException("Points must be positive", nameof(points));

            _currentScore = _currentScore.Add(points);
            _eventAggregator.Publish(new ScoreChangedEvent(_currentScore.Value));
        }

        /// <inheritdoc />
        public Score GetCurrentScore() => _currentScore;

        /// <inheritdoc />
        public Score GetRecordScore() => new(_recordManager.LoadRecord());

        /// <inheritdoc />
        public bool UpdateRecordIfNeeded()
        {
            var currentRecord = GetRecordScore();
            if (_currentScore.Value > currentRecord.Value)
            {
                _recordManager.CheckAndUpdateRecord(_currentScore.Value);
                _eventAggregator.Publish(new NewRecordSetEvent(_currentScore.Value));
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsNewRecord()
        {
            var currentRecord = GetRecordScore();
            return _currentScore.Value > currentRecord.Value;
        }
    }
}