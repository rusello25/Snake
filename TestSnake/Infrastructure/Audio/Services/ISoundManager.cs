namespace TestSnake.Infrastructure.Audio.Services
{
    public interface ISoundManager : IDisposable
    {
        bool IsSoundEnabled { get; }
        void SetSoundEnabled(bool enabled);
        Task PlayIntroLoopAsync(CancellationToken cancellationToken = default);
        Task PlayFanfareAsync(CancellationToken cancellationToken = default);
        void PlayEatSound();
        void PlayGameOverSound();
        void PlayLevelUpSound();
        void StopAllSounds();
    }
}