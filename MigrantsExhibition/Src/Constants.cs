using Microsoft.Xna.Framework;
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
        // Cell/Image Settings
        public const float CellSizeLayer1 = 60f; // Size for the first layer
        public const float CellSizeLayer2 = CellSizeLayer1 * 0.9f; // 10% smaller than layer 1

        // Transparency Settings
        public const float CellOpacity = 1.0f; // Opacity for live cells (1.0f = fully opaque)
        public const float LayerOpacityDifference = 0.8f; // Layer 2 cells are 80% opaque compared to Layer 1

        // Movement and Frame Rate
        public const int TargetFPS = 60; // Target frame rate
        public const double GenerationIntervalLow = 1.0; // One generation per second at <=10% sound
        public const double GenerationIntervalHigh = 0.0; // No generation at >=70% sound
        public const double GenerationIntervalIncreasePer10Sound = 0.07; // 7% increase per 10% sound

        // Sound Intensity Thresholds (1-100 scale)
        public const float SoundThresholdLow = 10f; // 10%
        public const float SoundThresholdMedium = 20f; // 20%
        public const float SoundThresholdHigh = 70f; // 70%

        // Cell Population Control
        public const float MinLiveCellsPercentage = 15f; // Minimum 15% live cells

        // Vibration Parameters
        public const float VibrationIntensityHigh = 10f; // Max vibration at >=70% sound

        // Layers
        public const int TotalLayers = 3; // Three layers

        // Fade-In Parameters
        public const float FadeInDuration = 2.0f; // Fade-in duration in seconds

        // GUI Parameters
        public const int SoundMeterWidth = 200; // Width of the sound intensity meter
        public const int SoundMeterHeight = 20; // Height of the sound intensity meter

        // Star Parameters
        public const float StarBaseSpeedMultiplier = 15f; // Base speed multiplier for stars based on depth
        public const float StarSoundIntensityMultiplier = 7f; // Speed increase based on sound intensity
        public const float StarVibrationMultiplier = 3f; // Vibration strength for stars

        // Additional Parameters for Depth Illusion
        public const float ShadowOffset = 5f; // Pixels to offset Layer 1 shadows
        public const float GlowSizeMultiplier = 1.2f; // Multiplier for glow size
        public const float Layer1Elevation = 5f; // Pixels to offset Layer 1 cells upward
        public const float ShadowOpacity = 0.5f; // Opacity for shadows
        public const float GlowOpacity = 0.5f; // Opacity for glows

        // Layer Colors
        public static readonly Color Layer1Color = Color.Cyan;
        public static readonly Color Layer2Color = Color.MediumPurple;
        public static readonly Color Layer3Color = Color.LightGreen;
    }
}
