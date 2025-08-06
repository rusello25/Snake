using TestSnake.Domain.ValueObjects;

namespace TestSnake.Application.Services
{
    /// <summary>
    /// Defines the contract for score management services.
    /// </summary>
    public interface IScoreService
    {
        /// <summary>
        /// Resets the current score to zero.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds a single point to the current score.
        /// </summary>
        void AddPoint();

        /// <summary>
        /// Adds multiple points to the current score.
        /// </summary>
        /// <param name="points">Number of points to add</param>
        void AddPoints(int points);

        /// <summary>
        /// Gets the current score.
        /// </summary>
        /// <returns>The current score value</returns>
        Score GetCurrentScore();

        /// <summary>
        /// Gets the highest recorded score.
        /// </summary>
        /// <returns>The record score value</returns>
        Score GetRecordScore();

        /// <summary>
        /// Updates the record if the current score is higher.
        /// </summary>
        /// <returns>True if a new record was set, false otherwise</returns>
        bool UpdateRecordIfNeeded();

        /// <summary>
        /// Determines if the current score is a new record.
        /// </summary>
        /// <returns>True if current score is a new record, false otherwise</returns>
        bool IsNewRecord();
    }
}