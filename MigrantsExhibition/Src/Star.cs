// Src/Star.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MigrantsExhibition.Src
{
    public class Star
    {
        public Vector2 Position { get; private set; }
        public float Size { get; private set; }
        public float Depth { get; private set; } // 0.0f (background) to 1.0f (foreground)

        private static Texture2D pixel;
        private GraphicsDevice graphicsDevice; // Private GraphicsDevice

        private static readonly Random random = new Random();

        public Star(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice; // Assign to the member variable

            Position = new Vector2(random.Next(0, graphicsDevice.Viewport.Width), random.Next(0, graphicsDevice.Viewport.Height));
            Size = (float)(random.NextDouble() * 2 + 1); // Size between 1 and 3
            Depth = (float)random.NextDouble(); // Random depth

            if (pixel == null)
            {
                pixel = new Texture2D(graphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Utils.LogInfo("Pixel texture created for stars.");
            }
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Adjust speed based on depth and sound intensity
            float baseSpeed = Constants.StarBaseSpeedMultiplier * Depth; // Background stars move slower
            float speed = baseSpeed + (soundIntensity * Constants.StarSoundIntensityMultiplier); // Increase speed with sound intensity
            Position += new Vector2(speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

            // Wrap around horizontally
            if (Position.X > graphicsDevice.Viewport.Width)
            {
                Position = new Vector2(0, Position.Y);
            }

            // Apply vibration based on sound intensity
            if (soundIntensity > 0.3f) // Threshold can be adjusted
            {
                float vibration = soundIntensity * Constants.StarVibrationMultiplier * (1.0f - Depth); // More vibration for higher layers
                float randomOffsetY = ((float)random.NextDouble() * 2 - 1) * vibration;
                Position = new Vector2(Position.X, Position.Y + randomOffsetY);

                Utils.LogInfo($"Star at ({Position.X}, {Position.Y}) is vibrating due to sound intensity.");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Define the destination rectangle based on size and depth
            Rectangle destinationRectangle = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)Size,
                (int)Size
            );

            // Adjust opacity based on depth
            float opacity = MathHelper.Lerp(0.3f, 1.0f, 1.0f - Depth); // Background stars are more transparent

            // Draw the star with adjusted opacity
            spriteBatch.Draw(
                pixel,
                destinationRectangle,
                null,
                Color.White * opacity,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1.0f - Depth // Layer depth
            );
        }

        // Dispose method to clean up the static pixel texture
        public static void DisposePixel()
        {
            if (pixel != null)
            {
                pixel.Dispose();
                pixel = null;
                Utils.LogInfo("Pixel texture disposed.");
            }
        }
    }
}
