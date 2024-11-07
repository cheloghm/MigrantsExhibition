using NAudio.Wave;
using System;

namespace MigrantsExhibition.Src
{
    public class AudioHandler : IDisposable
    {
        private WaveInEvent waveIn;
        private float maxVolume;
        private object lockObject = new object();

        public AudioHandler()
        {
            try
            {
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0; // Default microphone
                waveIn.WaveFormat = new WaveFormat(44100, 1); // Mono 44.1kHz

                waveIn.DataAvailable += OnDataAvailable;

                Utils.LogInfo($"AudioHandler initialized. Using device number: {waveIn.DeviceNumber}");
            }
            catch (Exception ex)
            {
                Utils.LogError($"Failed to initialize AudioHandler: {ex.Message}");
            }
        }

        public bool Start()
        {
            try
            {
                waveIn.StartRecording();
                Utils.LogInfo("AudioHandler started recording.");
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogError($"AudioHandler failed to start recording: {ex.Message}");
                return false;
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            float max = 0;

            // Interpret the audio data as 16-bit PCM samples
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                // Combine two bytes into a 16-bit sample
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                // Normalize the sample to a value between -1 and 1
                float sample32 = sample / 32768f;
                // Take the absolute value
                if (sample32 < 0) sample32 = -sample32;
                // Keep track of the maximum sample value
                if (sample32 > max) max = sample32;
            }

            lock (lockObject)
            {
                maxVolume = max;
            }
        }

        public void Update()
        {
            // Not needed in this implementation
        }

        public float GetSoundIntensity()
        {
            lock (lockObject)
            {
                return maxVolume * 100f; // Scale to 0-100%
            }
        }

        public void Dispose()
        {
            if (waveIn != null)
            {
                waveIn.DataAvailable -= OnDataAvailable;
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            Utils.LogInfo("AudioHandler disposed.");
        }
    }
}
