// Src/Constants.cs
using Microsoft.Xna.Framework;

namespace MigrantsExhibition.Src
{
    /// <summary>
    /// Holds all modifiable constants for the MigrantsExhibition project.
    /// Adjusting these values will change the behavior and appearance of the application.
    /// </summary>
    public static class Constants
    {
        // --- General Settings ---
        /// <summary>
        /// Target frames per second for the application.
        /// Example: 60
        /// </summary>
        public const int TargetFPS = 24;

        // --- Cell/Image Settings ---
        /// <summary>
        /// Base size of the cells.
        /// Example: 60f
        /// </summary>
        public const float BaseCellSize = 60f;

        /// <summary>
        /// Size of cells in Layer 1.
        /// Set to BaseCellSize by default.
        /// Example: BaseCellSize (60f)
        /// </summary>
        public const float CellSizeLayer1 = BaseCellSize;

        /// <summary>
        /// Size of cells in Layer 2.
        /// Set to BaseCellSize by default.
        /// Example: BaseCellSize (60f)
        /// </summary>
        public const float CellSizeLayer2 = BaseCellSize;

        /// <summary>
        /// Size of cells in Layer 3.
        /// Set to BaseCellSize by default.
        /// Example: BaseCellSize (60f)
        /// </summary>
        public const float CellSizeLayer3 = BaseCellSize;

        /// <summary>
        /// Opacity for live cells (1.0f = fully opaque).
        /// Example: 1.0f
        /// </summary>
        public const float CellOpacity = 1.0f;

        /// <summary>
        /// Opacity multiplier between layers.
        /// Layer 2 cells are multiplied by this value compared to Layer 1.
        /// Example: 0.8f (Layer 2 is 80% opaque compared to Layer 1)
        /// </summary>
        public const float LayerOpacityDifference = 0.8f;

        /// <summary>
        /// Colors for each layer of cells.
        /// Assign Color.Transparent if you don't want the layer to have a color.
        /// Examples:
        ///     Color.Cyan
        ///     Color.MediumPurple
        ///     Color.LightGreen
        ///     Color.Transparent
        ///     Color.White (to use original image colors)
        /// </summary>
        public static readonly Color Layer1Color = Color.White;
        public static readonly Color Layer2Color = Color.White;
        public static readonly Color Layer3Color = Color.White;
        // To use the original image colors, assign Color.White
        // To make a layer fully transparent (not visible), assign Color.Transparent

        // --- Sound Intensity Thresholds ---
        /// <summary>
        /// Sound intensity thresholds used to control behaviors.
        /// Values are on a 1-100 scale.
        /// Example: SoundThresholdLow = 10f
        /// </summary>
        public const float SoundThresholdLow = 10f;    // Below this, special behavior occurs
        public const float SoundThresholdHigh = 75f;   // Above this, special behavior occurs

        // --- Game of Life Settings ---
        /// <summary>
        /// Minimum percentage of live cells on the screen.
        /// Cells should not go below this percentage of the total possible cells.
        /// Example: 10f
        /// </summary>
        public const float MinLiveCellsPercentage = 10f; // 10%

        /// <summary>
        /// Maximum percentage of live cells on the screen.
        /// Cells should not exceed this percentage when sound is low.
        /// Example: 10f
        /// </summary>
        public const float MaxLiveCellsPercentage = 10f; // 10%

        /// <summary>
        /// The number of generations to wait before checking the live cell percentage.
        /// Example: 3
        /// </summary>
        public const int GenerationsToCheckPopulation = 2;

        /// <summary>
        /// Maximum number of cells allowed in the simulation to prevent performance issues.
        /// Example: 1000
        /// </summary>
        public const int MaxCells = 1000;

        // --- Vibration Parameters ---
        /// <summary>
        /// Vibration intensity for cells when sound intensity is high (>=75%).
        /// Increase this value to make cells shake more.
        /// Example: 10f
        /// </summary>
        public const float CellVibrationIntensityHigh = 10f;

        public const float CellVibrationIntensityMax = 20f; // Maximum vibration intensity at 100% sound intensity

        /// <summary>
        /// Vibration intensity for cells when sound intensity is between low and high thresholds.
        /// Example: 5f
        /// </summary>
        public const float CellVibrationIntensityMedium = 5f;

        /// <summary>
        /// Vibration intensity for cells when sound intensity is low (<10%).
        /// Example: 0f (No vibration)
        /// </summary>
        public const float CellVibrationIntensityLow = 0f; // No vibration

        // --- Fade-In Parameters ---
        /// <summary>
        /// Duration of the initial fade-in effect in seconds.
        /// Example: 2.0f
        /// </summary>
        public const float FadeInDuration = 2.0f;

        // --- GUI Parameters ---
        /// <summary>
        /// Width of the sound intensity meter in pixels.
        /// Example: 200
        /// </summary>
        public const int SoundMeterWidth = 200;

        /// <summary>
        /// Height of the sound intensity meter in pixels.
        /// Example: 20
        /// </summary>
        public const int SoundMeterHeight = 20;

        // --- Star Parameters ---
        /// <summary>
        /// Base speed multiplier for stars based on depth.
        /// Adjust this to change the base speed of stars.
        /// Example: 15f
        /// </summary>
        public const float StarBaseSpeedMultiplier = 15f;

        /// <summary>
        /// Speed increase for stars based on sound intensity.
        /// Example: 7f
        /// </summary>
        public const float StarSoundIntensityMultiplier = 7f;

        /// <summary>
        /// Vibration strength for stars.
        /// Example: 3f
        /// </summary>
        public const float StarVibrationMultiplier = 3f;

        // --- Depth Illusion Parameters ---
        /// <summary>
        /// Pixels to offset Layer 1 shadows.
        /// Example: 5f
        /// </summary>
        public const float ShadowOffset = 5f;

        /// <summary>
        /// Opacity for shadows (0.0f = fully transparent, 1.0f = fully opaque).
        /// Example: 0.5f
        /// </summary>
        public const float ShadowOpacity = 0.5f;

        // --- Other Parameters ---
        /// <summary>
        /// Total number of layers used in the application.
        /// Example: 3
        /// </summary>
        public const int TotalLayers = 3;

        /// <summary>
        /// Initial number of cells to start with.
        /// Example: 50
        /// </summary>
        public const int InitialCellCount = 50;

        /// <summary>
        /// Number of stars to create per layer.
        /// Example: 100
        /// </summary>
        public const int StarCountPerLayer = 100;
    }
}
