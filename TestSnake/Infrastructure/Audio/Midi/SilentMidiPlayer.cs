namespace TestSnake.Infrastructure.Audio.Midi
{   
    public class SilentMidiPlayer : IMidiPlayer
    {
        public void Dispose() 
        { 
            GC.SuppressFinalize(this);
        }
        public void PlayEat() { }
        public Task PlayFanfareAsync(CancellationToken token) => Task.CompletedTask;
        public void PlayGameOver() { }
        public void PlayIntroLoop(CancellationToken token) { }
        public void PlayMelody() { }
        public void PlayNote(int note, int velocity = 127, int durationMs = 200, int channel = 1) { }
        public void PlayStart() { }
        public void StopGameOverMelody() { }
        public void StopMelody() { }
        public void PlayLevelUp() { }
    }
}