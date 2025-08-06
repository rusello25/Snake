using NAudio.Midi;

namespace TestSnake.Infrastructure.Audio.Midi
{
    public class MidiPlayer : IMidiPlayer
    {
        private MidiOut? midiOut;
        private bool initialized = false;
        private readonly Task? gameOverMelodyTask = null;
        private CancellationTokenSource? melodyCts = null;
        private CancellationTokenSource? gameOverCts = null;
        private readonly int midiDeviceIndex;
        private readonly object midiLock = new();
        private bool disposed = false;
        private const int MelodyChannel = 1;
        private const int EffectChannel = 2;

        public MidiPlayer(int midiDeviceIndex = 0)
        {
            this.midiDeviceIndex = midiDeviceIndex;
            try
            {
                midiOut = new MidiOut(midiDeviceIndex);
                initialized = true;
            }
            catch
            {
                midiOut = null;
                initialized = false;
            }
        }

        private void SendMidi(Action<MidiOut> action)
        {
            lock (midiLock)
            {
                if (!initialized || midiOut == null || disposed) return;
                try { action(midiOut); }
                catch (ObjectDisposedException) { }
            }
        }

        private void PlayNoteSync(int note, int velocity, int durationMs, int channel)
        {
            SendMidi(mo => mo.Send(MidiMessage.StartNote(note, velocity, channel).RawData));
            Thread.Sleep(durationMs);
            SendMidi(mo => mo.Send(MidiMessage.StopNote(note, 0, channel).RawData));
        }

        public void PlayNote(int note, int velocity = 127, int durationMs = 200, int channel = MelodyChannel)
        {
            StopGameOverMelody();
            Task.Run(() => PlayNoteSync(note, velocity, durationMs, channel), CancellationToken.None);
        }

        public void PlayEat()
        {
            StopGameOverMelody();
            Task.Run(() =>
            {
                PlayNoteSync(76, 127, 50, EffectChannel);
                PlayNoteSync(79, 127, 50, EffectChannel);
            }, CancellationToken.None);
        }

        public void PlayStart()
        {
            StopGameOverMelody();
            melodyCts?.Cancel();
            melodyCts = new CancellationTokenSource();
            var token = melodyCts.Token;
            Task.Run(() =>
            {
                int t = 90;
                int t2 = 180;
                PlayNoteSync(72, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(76, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(79, 127, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(60);
                PlayNoteSync(76, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(79, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(84, 127, t2, MelodyChannel); if (token.IsCancellationRequested) return;
            }, token);
        }

        public void PlayGameOver()
        {
            StopMelody();
            StopGameOverMelody();
            gameOverCts = new CancellationTokenSource();
            var token = gameOverCts.Token;
            Task.Run(() =>
            {
                int tempo = 300;
                int pause = 100;
                while (!token.IsCancellationRequested)
                {
                    PlayNoteSync(48, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    PlayNoteSync(47, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    PlayNoteSync(45, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    PlayNoteSync(43, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    Thread.Sleep(pause);
                    PlayNoteSync(43, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    PlayNoteSync(47, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    PlayNoteSync(50, 127, tempo, MelodyChannel); if (token.IsCancellationRequested) break;
                    Thread.Sleep(pause);
                    PlayNoteSync(36, 127, tempo * 2, MelodyChannel); if (token.IsCancellationRequested) break;
                }
            }, token);
        }

        public void StopGameOverMelody()
        {
            gameOverCts?.Cancel();
            gameOverCts = null;
        }

        public void PlayMelody()
        {
            StopGameOverMelody();
            melodyCts?.Cancel();
            melodyCts = new CancellationTokenSource();
            var token = melodyCts.Token;
            Task.Run(() =>
            {
                int t = 160;
                int t2 = 320;
                int pause = 80;
                
                PlayNoteSync(60, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(71, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(72, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(69, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(59, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(65, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(68, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(71, 100, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(68, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(65, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(59, 100, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(60, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(69, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(76, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(72, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
            }, token);
        }

        public void StopMelody()
        {
            melodyCts?.Cancel();
        }

        public void PlayIntroLoop(CancellationToken token)
        {
            StopGameOverMelody();
            int t = 160;
            int t2 = 320;
            int pause = 80;
            do
            {
                PlayNoteSync(60, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(71, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(72, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(69, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(59, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(65, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(68, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(71, 100, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(68, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(65, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(62, 100, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(59, 100, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(60, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(64, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(69, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
                PlayNoteSync(76, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(74, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(72, 110, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(67, 110, t2, MelodyChannel); if (token.IsCancellationRequested) return;
                Thread.Sleep(pause);
            } while (!token.IsCancellationRequested);
        }

        public Task PlayFanfareAsync(CancellationToken token)
        {
            StopGameOverMelody();
            return Task.Run(() =>
            {
                int t = 90;
                int t2 = 180;
                PlayNoteSync(84, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(83, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(81, 127, t, MelodyChannel); if (token.IsCancellationRequested) return;
                PlayNoteSync(79, 127, t2, MelodyChannel); if (token.IsCancellationRequested) return;
            }, token);
        }

        public void PlayLevelUp()
        {
            StopGameOverMelody();
            Task.Run(() =>
            {
                int t = 80;
                int t2 = 120;
                
                PlayNoteSync(72, 127, t, MelodyChannel);
                PlayNoteSync(76, 127, t, MelodyChannel);
                PlayNoteSync(79, 127, t, MelodyChannel);
                PlayNoteSync(84, 127, t2, MelodyChannel);
            }, CancellationToken.None);
        }

        public void Dispose()
        {
            lock (midiLock)
            {
                if (disposed) return;
                disposed = true;
                melodyCts?.Cancel();
                gameOverMelodyTask?.Wait(1000);
                midiOut?.Dispose();
                midiOut = null;
                initialized = false;
                GC.SuppressFinalize(this);
            }
        }
    }
}