// Src/GUI.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MigrantsExhibition.Src
{
    public class GUI : IDisposable
    {
        public SpriteFont Font { get; private set; }
        private Vector2 position;
        private Color textColor;
        private float soundIntensity; // 1-100 scale

        private Texture2D rectTexture;
        private Texture2D fillTexture;
        public Texture2D OverlayTexture { get; private set; }

        public GUI(ContentManager content, GraphicsDevice graphicsDevice)
        {
            try
            {
                Font = content.Load<SpriteFont>("arial"); // Ensure the name matches exactly without extension
                position = new Vector2(10, 10); // Top-left corner
                textColor = Color.White;
                soundIntensity = 0f;

                // Create 1x1 textures for rectangles using graphicsDevice
                rectTexture = new Texture2D(graphicsDevice, 1, 1);
                rectTexture.SetData(new[] { Color.Gray });

                fillTexture = new Texture2D(graphicsDevice, 1, 1);
                fillTexture.SetData(new[] { Color.Green });

                // Create an overlay texture for initial fade-in
                OverlayTexture = new Texture2D(graphicsDevice, 1, 1);
                OverlayTexture.SetData(new[] { Color.Black });

                Utils.LogInfo("GUI initialized successfully.");
            }
            catch (ContentLoadException ex)
            {
                Utils.LogError($"Failed to load SpriteFont 'arial': {ex.Message}");
                throw; // Re-throw to handle higher up or terminate
            }
            catch (Exception ex)
            {
                Utils.LogError($"Unexpected error in GUI constructor: {ex.Message}");
                throw;
            }
        }

        public void Update(float soundIntensity)
        {
            this.soundIntensity = soundIntensity;
            // Additional GUI updates can be implemented here
            Utils.LogInfo($"GUI updated with Sound Intensity: {soundIntensity:F2}%");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Example: Draw sound intensity meter
            Vector2 meterPosition = new Vector2(10, 30);
            float meterWidth = Constants.SoundMeterWidth;
            float meterHeight = Constants.SoundMeterHeight;
            float normalizedSound = MathHelper.Clamp(soundIntensity / 100f, 0f, 1f);
            float fillWidth = meterWidth * normalizedSound;

            // Draw meter background
            spriteBatch.Draw(rectTexture, new Rectangle((int)meterPosition.X, (int)meterPosition.Y, (int)meterWidth, (int)meterHeight), Color.Gray);

            // Draw meter fill
            spriteBatch.Draw(fillTexture, new Rectangle((int)meterPosition.X, (int)meterPosition.Y, (int)fillWidth, (int)meterHeight), Color.Green);

            // Draw sound intensity text
            string soundText = $"Sound Intensity: {soundIntensity:F2}%";
            spriteBatch.DrawString(Font, soundText, new Vector2(10, 60), textColor);

            Utils.LogInfo($"GUI drawn with Sound Intensity: {soundIntensity:F2}%");
        }

        public void Dispose()
        {
            rectTexture?.Dispose();
            fillTexture?.Dispose();
            OverlayTexture?.Dispose();
            // Do not dispose of Font here if it's managed by ContentManager
            Utils.LogInfo("GUI disposed.");
        }
    }
}
