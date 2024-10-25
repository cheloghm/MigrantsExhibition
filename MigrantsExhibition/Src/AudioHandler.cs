// Src/AudioHandler.cs
using NAudio.Wave;
using System;

namespace MigrantsExhibition.Src
{
    public class AudioHandler : IDisposable
    {
        private WaveInEvent waveIn;
        private float soundIntensity;

        public bool Start()
        {
            try
            {
                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(44100, 1) // 44.1kHz, Mono
                };
                waveIn.DataAvailable += OnDataAvailable;
                waveIn.StartRecording();
                Utils.LogInfo("Audio recording started.");
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogError($"Failed to start audio recording: {ex.Message}");
                return false;
            }
        }

        public void Update()
        {
            // Currently, no additional processing needed.
            // soundIntensity is updated via the DataAvailable event.
        }

        public void Stop()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
                Utils.LogInfo("Audio recording stopped.");
            }
        }

        public float GetSoundIntensity()
        {
            return soundIntensity;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            // Calculate RMS (Root Mean Square) to determine sound intensity
            float sum = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = BitConverter.ToInt16(e.Buffer, index);
                float sample32 = sample / 32768f; // Normalize to [-1,1]
                sum += sample32 * sample32;
            }

            float rms = (float)Math.Sqrt(sum / (e.BytesRecorded / 2));
            soundIntensity = rms;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
