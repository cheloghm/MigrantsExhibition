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
        public float Depth { get; private set; } // 0.0f (farthest) to 1.0f (closest)
        public int Layer { get; private set; } // 1, 2, 3

        private GraphicsDevice graphicsDevice;
        private static Random random = new Random();
        private static Texture2D pixel;

        public Star(GraphicsDevice graphicsDevice, int layer)
        {
            this.graphicsDevice = graphicsDevice;
            Layer = layer;

            // Initialize Position
            Position = new Vector2(random.Next(0, graphicsDevice.Viewport.Width), random.Next(0, graphicsDevice.Viewport.Height));

            // Initialize Size based on Layer
            Size = layer switch
            {
                1 => (float)(random.NextDouble() * 1 + 0.5), // Smaller size for farthest
                2 => (float)(random.NextDouble() * 1.5 + 1.0),
                3 => (float)(random.NextDouble() * 2 + 1.5), // Larger size for nearest
                _ => 1.0f,
            };

            // Initialize Depth based on Layer
            Depth = layer switch
            {
                1 => 0.2f,
                2 => 0.5f,
                3 => 0.8f,
                _ => 0.5f,
            };

            // Initialize Pixel Texture
            if (pixel == null)
            {
                pixel = new Texture2D(graphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Utils.LogInfo("Pixel texture created for stars.");
            }

            Utils.LogInfo($"Star created at ({Position.X}, {Position.Y}) in Layer {Layer} with Depth {Depth}.");
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Adjust speed based on depth and sound intensity
            float baseSpeed = Constants.StarBaseSpeedMultiplier * Depth; // Background stars move slower
            float speed = baseSpeed + (soundIntensity / 100f * Constants.StarSoundIntensityMultiplier); // Increase speed with sound intensity
            Position += new Vector2(speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

            // Wrap around horizontally
            if (Position.X > graphicsDevice.Viewport.Width)
            {
                Position = new Vector2(0, Position.Y);
            }

            // Apply vibration based on sound intensity
            if (soundIntensity >= Constants.SoundThresholdHigh) // Threshold can be adjusted
            {
                Vibrate(soundIntensity, gameTime);
            }
        }

        private void Vibrate(float soundIntensity, GameTime gameTime)
        {
            float vibrationAmount = (soundIntensity / 100f) * Constants.StarVibrationMultiplier;
            float offsetX = ((float)random.NextDouble() * 2 - 1) * vibrationAmount * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float offsetY = ((float)random.NextDouble() * 2 - 1) * vibrationAmount * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2(offsetX, offsetY);

            // Ensure the star stays within screen bounds
            WrapAround();
        }

        private void WrapAround()
        {
            float leftBound = 0f;
            float rightBound = graphicsDevice.Viewport.Width;
            float topBound = 0f;
            float bottomBound = graphicsDevice.Viewport.Height;

            // Horizontal wrap-around
            if (Position.X < leftBound)
            {
                Position = new Vector2(Position.X + rightBound, Position.Y);
            }
            if (Position.X > rightBound)
            {
                Position = new Vector2(Position.X - rightBound, Position.Y);
            }

            // Vertical wrap-around
            if (Position.Y < topBound)
            {
                Position = new Vector2(Position.X, Position.Y + bottomBound);
            }
            if (Position.Y > bottomBound)
            {
                Position = new Vector2(Position.X, Position.Y - bottomBound);
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
