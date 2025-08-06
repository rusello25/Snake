namespace TestSnake.Infrastructure.Audio.Midi
{
    public interface IMidiPlayer : IDisposable
    {
        void PlayNote(int note, int velocity = 127, int durationMs = 200, int channel = 1);
        void PlayEat();
        void PlayStart();
        void PlayGameOver();
        void StopGameOverMelody();
        void PlayMelody();
        void StopMelody();
        void PlayIntroLoop(CancellationToken token);
        Task PlayFanfareAsync(CancellationToken token);
        void PlayLevelUp();
    }
}