// Src/Cell.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MigrantsExhibition.Src
{
    public class Cell
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }
        public float Scale { get; set; }
        public float Depth { get; private set; } // 0.0f (foreground) to 1.0f (background)

        private GraphicsDevice graphicsDevice;

        private const float MaxSize = Constants.CellSize; // Use centralized cell size
        private float originalWidth;
        private float originalHeight;

        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        public float BaseSpeed { get; private set; } = Constants.BaseSpeed; // Further reduced base speed
        public float MaxSpeed { get; private set; } = Constants.MaxSpeed;  // Further reduced maximum speed

        private static readonly Random random = new Random();

        // Animation Flags
        public bool IsBorn { get; set; } = true; // Indicates if the cell is newly born
        public bool IsDying { get; private set; } = false; // Indicates if the cell is dying

        private float birthScale = 0.0f; // Initial scale for the zoom-in effect
        private const float BirthScaleIncrement = Constants.BirthScaleIncrement; // Zoom-in increment per update

        private float deathScale = 1.0f; // Initial scale for the zoom-out effect
        private const float DeathScaleDecrement = Constants.DeathScaleDecrement; // Zoom-out decrement per update

        public Cell(Texture2D texture, Vector2 position, Vector2 direction, float speed, GraphicsDevice graphicsDevice, float depth = 0.5f)
        {
            Texture = texture;
            Position = position;
            Direction = direction;
            Speed = speed;
            BaseSpeed = speed; // Ensure BaseSpeed reflects the initial speed
            MaxSpeed = Constants.MaxSpeed; // Use centralized max speed
            Scale = 0.0f; // Start with scale 0 for zoom-in
            this.graphicsDevice = graphicsDevice;
            Depth = MathHelper.Clamp(depth, 0.0f, 1.0f); // Ensure depth is between 0 and 1

            originalWidth = Texture.Width;
            originalHeight = Texture.Height;
            Utils.LogInfo($"Cell created at ({Position.X}, {Position.Y}) with depth {Depth}.");
        }

        public void Update(GameTime gameTime, float soundIntensity, bool isAboveNormal, float midSoundIntensity)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Adjust Speed based on soundIntensity
            Speed = MathHelper.Lerp(BaseSpeed, BaseSpeed + (MaxSpeed - BaseSpeed) * 0.5f, soundIntensity);

            Position += Direction * Speed * deltaTime;

            // Screen wrapping
            WrapAround();

            // Vibration and scaling based on sound intensity
            if (soundIntensity > 0.1f) // Adjust threshold as needed
            {
                // Vibration effect proportional to soundIntensity and layer depth
                float vibration = soundIntensity * Constants.VibrationMultiplier * (1.0f - Depth); // Use centralized vibration multiplier
                float oscillation = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 20) * vibration;

                // Apply vibration
                float newX = Position.X + oscillation;
                float newY = Position.Y + oscillation;
                Position = new Vector2(newX, newY);

                // Scaling effect based on depth
                float targetScale = 1.0f + (soundIntensity * 0.5f); // Adjusted for subtle scaling

                // Calculate the scale to ensure the largest dimension is CellSize
                float scaleX = (MaxSize / originalWidth) * (1.0f - Depth) + 1.0f; // Foreground cells can scale more
                float scaleY = (MaxSize / originalHeight) * (1.0f - Depth) + 1.0f;
                float maxScale = Math.Min(scaleX, scaleY); // To maintain aspect ratio

                // Clamp the targetScale to not exceed maxScale
                Scale = MathHelper.Clamp(targetScale, 1.0f, maxScale);

                Utils.LogInfo($"Cell at ({Position.X}, {Position.Y}) updated with Scale={Scale}.");
            }
            else
            {
                // Smoothly return to original scale
                Scale = MathHelper.Lerp(Scale, 1.0f, 0.05f);
            }

            // Layer-based vibration scaling
            if (isAboveNormal && soundIntensity >= midSoundIntensity)
            {
                // Apply additional random vibration influenced by depth
                float vibrationAmplitude = (soundIntensity - midSoundIntensity) * (Constants.LayerVibrationMultiplier * (1.0f - Depth)); // Use centralized layer vibration multiplier
                float randomOffsetX = ((float)random.NextDouble() * 2 - 1) * vibrationAmplitude;
                float randomOffsetY = ((float)random.NextDouble() * 2 - 1) * vibrationAmplitude;
                Position += new Vector2(randomOffsetX, randomOffsetY);

                Utils.LogInfo($"Cell at ({Position.X}, {Position.Y}) is vibrating due to high sound intensity.");
            }
            else
            {
                // No additional vibration; ensure position remains stable
            }

            // Handle zoom-in animation for newly born cells
            if (IsBorn)
            {
                birthScale += BirthScaleIncrement;
                if (birthScale >= 1.0f)
                {
                    birthScale = 1.0f;
                    IsBorn = false; // Animation complete
                }
                Scale = MathHelper.Lerp(Scale, 1.0f, birthScale);
            }

            // Handle zoom-out animation for dying cells
            if (IsDying)
            {
                deathScale -= DeathScaleDecrement;
                if (deathScale <= 0.0f)
                {
                    deathScale = 0.0f;
                    // Cell should be removed from the GameOfLife list
                    // This will be handled in GameOfLife.cs
                }
                Scale = MathHelper.Lerp(Scale, 0.0f, DeathScaleDecrement);
            }
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
                // Optionally, spawn a new cell entering from the opposite side
            }
            if (Position.X > rightBound)
            {
                Position = new Vector2(Position.X - rightBound, Position.Y);
                // Optionally, spawn a new cell entering from the opposite side
            }

            // Vertical wrap-around
            if (Position.Y < topBound)
            {
                Position = new Vector2(Position.X, Position.Y + bottomBound);
                // Optionally, spawn a new cell entering from the opposite side
            }
            if (Position.Y > bottomBound)
            {
                Position = new Vector2(Position.X, Position.Y - bottomBound);
                // Optionally, spawn a new cell entering from the opposite side
            }
        }

        public void Die()
        {
            IsDying = true;
        }

        // Draw the cell
        public void Draw(SpriteBatch spriteBatch)
        {
            // Define the destination rectangle with desired size (CellSize x CellSize)
            Rectangle destinationRectangle = new Rectangle(
                (int)Position.X - (int)(Constants.CellSize / 2), // Center the cell
                (int)Position.Y - (int)(Constants.CellSize / 2),
                (int)Constants.CellSize, // Width
                (int)Constants.CellSize  // Height
            );

            // Apply birth scaling
            float drawScale = Scale;
            if (IsBorn)
            {
                drawScale *= birthScale;
            }

            // Adjust opacity based on depth
            float opacity = MathHelper.Lerp(0.5f, 1.0f, 1.0f - Depth); // Closer layers are more opaque

            // Draw the texture scaled to CellSize with zoom-in/out effect and adjusted opacity
            spriteBatch.Draw(
                Texture,
                destinationRectangle,
                null,
                Color.White * opacity,
                0f,
                Vector2.Zero, // Origin is top-left since we're using a destination rectangle
                SpriteEffects.None,
                Depth // Use Depth as layer depth
            );
        }
    }
}
