using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Src/Constants.cs
namespace MigrantsExhibition.Src
{
    public static class Constants
    {
        // Cell/Image Size
        public const float CellSize = 40f; // Size of each cell in pixels

        // Movement Speeds
        public const float BaseSpeed = 20f; // Base speed for cells
        public const float MaxSpeed = 60f;  // Maximum speed for cells

        // Zoom Effects
        public const float BirthScaleIncrement = 0.02f; // Zoom-in increment per update
        public const float DeathScaleDecrement = 0.02f; // Zoom-out decrement per update

        // Sound Intensity Thresholds
        public const float NormalSoundIntensity = 0.5f; // Normal sound intensity threshold
        public const float MidSoundIntensity = 0.7f;    // Mid-level sound intensity threshold

        // Vibration Parameters
        public const float VibrationMultiplier = 5f;      // Vibration strength based on sound intensity
        public const float LayerVibrationMultiplier = 10f; // Additional vibration for higher layers

        // Game of Life Parameters
        public const double GenerationInterval = 0.25; // Time between generations in seconds (4 per second)

        // Star Parameters
        public const float StarBaseSpeedMultiplier = 10f; // Base speed multiplier for stars based on depth
        public const float StarSoundIntensityMultiplier = 5f; // Speed increase based on sound intensity
        public const float StarVibrationMultiplier = 2f;   // Vibration strength for stars

        // GUI Parameters
        public const int SoundMeterWidth = 200;
        public const int SoundMeterHeight = 20;
    }
}

