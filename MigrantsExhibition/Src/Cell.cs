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
        public int Layer { get; private set; } // 1, 2, 3

        private GraphicsDevice graphicsDevice;
        private static Texture2D shadowTexture;

        private static readonly Random random = new Random();

        // Animation Flags
        public bool IsBorn { get; set; } = true; // Indicates if the cell is newly born
        public bool IsDying { get; private set; } = false; // Indicates if the cell is dying

        private float birthScale = 0.0f; // Initial scale for the zoom-in effect
        private const float BirthScaleIncrement = 0.02f; // Zoom-in increment per update

        private float deathScale = 1.0f; // Initial scale for the zoom-out effect
        private const float DeathScaleDecrement = 0.02f; // Zoom-out decrement per update

        public Cell(Texture2D texture, Vector2 position, Vector2 direction, int layer, GraphicsDevice graphicsDevice, float depth = 0.5f)
        {
            Texture = texture;
            Position = position;
            Direction = direction;
            Layer = layer;
            Depth = MathHelper.Clamp(depth, 0.0f, 1.0f); // Ensure depth is between 0 and 1
            this.graphicsDevice = graphicsDevice;

            // Assign speed based on layer
            switch (Layer)
            {
                case 1:
                    Speed = 0f; // No inherent movement; movement simulated via cell generation
                    break;
                case 2:
                    Speed = 0f; // No inherent movement
                    break;
                case 3:
                    Speed = 0f; // No inherent movement
                    break;
                default:
                    Speed = 0f;
                    break;
            }

            Scale = 0.0f; // Start with scale 0 for zoom-in

            // Initialize shadow texture if Layer 1 and not already initialized
            if (Layer == 1 && shadowTexture == null)
            {
                shadowTexture = CreateShadowTexture(graphicsDevice, 20); // 20px shadow
                Utils.LogInfo("Shadow texture created for Layer 1 cells.");
            }

            Utils.LogInfo($"Cell created at ({Position.X}, {Position.Y}) in Layer {Layer} with Depth {Depth}.");
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            // Handle vibration based on sound intensity
            if (soundIntensity >= Constants.SoundThresholdHigh)
            {
                Vibrate(soundIntensity, deltaTime);
            }
        }

        private void Vibrate(float soundIntensity, float deltaTime)
        {
            float vibrationAmount = (soundIntensity / 100f) * Constants.VibrationIntensityHigh * deltaTime;
            float offsetX = ((float)random.NextDouble() * 2 - 1) * vibrationAmount;
            float offsetY = ((float)random.NextDouble() * 2 - 1) * vibrationAmount;
            Position += new Vector2(offsetX, offsetY);

            // Ensure the cell stays within screen bounds
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

        public void Die()
        {
            IsDying = true;
        }

        // Create a simple circular shadow texture
        private Texture2D CreateShadowTexture(GraphicsDevice graphicsDevice, int diameter)
        {
            Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            float radius = diameter / 2f;
            float radiussq = radius * radius;

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    int index = y * diameter + x;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (pos.LengthSquared() <= radiussq)
                    {
                        float alpha = 1.0f - (pos.Length() / radius); // Fade out towards edges
                        colorData[index] = new Color(0, 0, 0, alpha * 0.5f); // Semi-transparent black
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        // Draw the cell with optional shadow
        public void Draw(SpriteBatch spriteBatch)
        {
            // Determine cell size based on layer
            float currentCellSize = Layer switch
            {
                1 => Constants.CellSizeLayer1,
                2 => Constants.CellSizeLayer2,
                3 => Constants.CellSizeLayer2 * 0.9f, // Adjust as needed
                _ => Constants.CellSizeLayer1,
            };

            // Define the destination rectangle with desired size
            Rectangle destinationRectangle = new Rectangle(
                (int)Position.X - (int)(currentCellSize / 2),
                (int)Position.Y - (int)(currentCellSize / 2),
                (int)currentCellSize,
                (int)currentCellSize
            );

            // Apply birth scaling
            float drawScale = Scale;
            if (IsBorn)
            {
                drawScale *= birthScale;
            }

            // Adjust opacity based on constants and layer
            Color cellColor = Layer switch
            {
                1 => Constants.Layer1Color,
                2 => Constants.Layer2Color,
                3 => Constants.Layer3Color,
                _ => Color.White,
            };

            float opacity = Layer switch
            {
                1 => Constants.CellOpacity, // Fully opaque
                2 => Constants.CellOpacity * Constants.LayerOpacityDifference, // 80% opaque
                3 => Constants.CellOpacity * Constants.LayerOpacityDifference * 0.8f, // 64% opaque
                _ => Constants.CellOpacity,
            };

            // Draw shadow for Layer 1 cells
            if (Layer == 1 && shadowTexture != null)
            {
                Rectangle shadowRect = new Rectangle(
                    destinationRectangle.X + (int)Constants.ShadowOffset, // Offset shadow slightly
                    destinationRectangle.Y + (int)Constants.ShadowOffset,
                    (int)(currentCellSize * 1.1f), // Slightly larger
                    (int)(currentCellSize * 1.1f)
                );
                spriteBatch.Draw(
                    shadowTexture,
                    shadowRect,
                    null,
                    Color.Black * Constants.ShadowOpacity, // Semi-transparent shadow
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    Depth + 0.001f // Ensure shadow is slightly behind the cell
                );
            }

            // Draw the texture scaled to CellSize with zoom-in/out effect and adjusted opacity
            spriteBatch.Draw(
                Texture,
                destinationRectangle,
                null,
                cellColor * opacity,
                0f,
                Vector2.Zero, // Origin is top-left since we're using a destination rectangle
                SpriteEffects.None,
                Depth // Use Depth as layer depth
            );
        }
    }
}
