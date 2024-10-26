// Src/AudioHandler.cs
using NAudio.CoreAudioApi;
using System;

namespace MigrantsExhibition.Src
{
    public class AudioHandler : IDisposable
    {
        private MMDeviceEnumerator deviceEnumerator;
        private MMDevice device;
        private AudioMeterInformation meter;

        public AudioHandler()
        {
            deviceEnumerator = new MMDeviceEnumerator();
            device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            meter = device.AudioMeterInformation;
            Utils.LogInfo("AudioHandler initialized.");
        }

        public bool Start()
        {
            try
            {
                // Initialization successful
                Utils.LogInfo("AudioHandler started successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogError($"AudioHandler failed to start: {ex.Message}");
                return false;
            }
        }

        public void Update()
        {
            // For future enhancements if needed
        }

        public float GetSoundIntensity()
        {
            // Get peak value of the first channel and map to 1-100 scale
            float peak = meter.MasterPeakValue * 100f; // Map to 0-100 scale
            return peak; // Returns a value between 0.0f and 100f
        }

        public void Dispose()
        {
            meter = null;
            device.Dispose();
            deviceEnumerator.Dispose();
            Utils.LogInfo("AudioHandler disposed.");
        }
    }
}
