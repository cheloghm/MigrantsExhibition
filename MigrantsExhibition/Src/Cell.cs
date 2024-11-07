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
        public float Scale { get; set; } = 1.0f; // Cells are always drawn at full scale
        public float Depth { get; private set; } // 0.0f (foreground) to 1.0f (background)
        public int Layer { get; private set; } // 1, 2, 3

        private GraphicsDevice graphicsDevice;
        private static Texture2D shadowTexture;

        private static readonly Random random = new Random();

        public Cell(Texture2D texture, Vector2 position, Vector2 direction, int layer, GraphicsDevice graphicsDevice, float depth = 0.5f)
        {
            Texture = texture;
            Position = position;
            Direction = direction;
            Layer = layer;
            Depth = MathHelper.Clamp(depth, 0.0f, 1.0f); // Ensure depth is between 0 and 1
            this.graphicsDevice = graphicsDevice;

            // Assign speed based on layer
            Speed = 0f; // No inherent movement

            // Cells are always at full scale
            Scale = 1.0f;

            // Initialize shadow texture if Layer 1 and not already initialized
            if (Layer == 1 && shadowTexture == null)
            {
                shadowTexture = CreateShadowTexture(graphicsDevice, 20); // 20px shadow
            }
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Handle vibration based on sound intensity
            if (soundIntensity >= Constants.SoundThresholdHigh)
            {
                Vibrate(soundIntensity, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else if (soundIntensity >= Constants.SoundThresholdLow && soundIntensity < Constants.SoundThresholdHigh)
            {
                // Vibration intensity scales between low and medium thresholds
                Vibrate(soundIntensity, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                // No vibration when sound intensity is below low threshold
            }
        }

        // Src/Cell.cs
        private void Vibrate(float soundIntensity, float deltaTime)
        {
            float vibrationIntensity = 0f;

            if (soundIntensity >= Constants.SoundThresholdHigh)
            {
                // Vibration intensity increases with sound intensity
                float excessSound = soundIntensity - Constants.SoundThresholdHigh;
                float normalizedExcessSound = excessSound / (100f - Constants.SoundThresholdHigh);
                vibrationIntensity = Constants.CellVibrationIntensityHigh + normalizedExcessSound * (Constants.CellVibrationIntensityMax - Constants.CellVibrationIntensityHigh);
            }
            else if (soundIntensity >= Constants.SoundThresholdLow)
            {
                // Vibration intensity scales between low and high thresholds
                float normalizedSound = (soundIntensity - Constants.SoundThresholdLow) / (Constants.SoundThresholdHigh - Constants.SoundThresholdLow);
                vibrationIntensity = normalizedSound * Constants.CellVibrationIntensityMedium;
            }

            // Apply vibration
            float vibrationAmount = (vibrationIntensity / 100f) * deltaTime * 100f;
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
                        colorData[index] = new Color(0, 0, 0, alpha * Constants.ShadowOpacity);
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
                3 => Constants.CellSizeLayer3,
                _ => Constants.CellSizeLayer1,
            };

            // Define the destination rectangle with desired size
            Rectangle destinationRectangle = new Rectangle(
                (int)(Position.X - (currentCellSize * Scale) / 2),
                (int)(Position.Y - (currentCellSize * Scale) / 2),
                (int)(currentCellSize * Scale),
                (int)(currentCellSize * Scale)
            );

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
                1 => Constants.CellOpacity,
                2 => Constants.CellOpacity * Constants.LayerOpacityDifference,
                3 => Constants.CellOpacity * Constants.LayerOpacityDifference * Constants.LayerOpacityDifference,
                _ => Constants.CellOpacity,
            };

            // Draw shadow for Layer 1 cells
            if (Layer == 1 && shadowTexture != null)
            {
                Rectangle shadowRect = new Rectangle(
                    destinationRectangle.X + (int)Constants.ShadowOffset,
                    destinationRectangle.Y + (int)Constants.ShadowOffset,
                    (int)(destinationRectangle.Width * 1.1f),
                    (int)(destinationRectangle.Height * 1.1f)
                );
                spriteBatch.Draw(
                    shadowTexture,
                    shadowRect,
                    null,
                    Color.Black * Constants.ShadowOpacity,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    Depth + 0.001f
                );
            }

            // Draw the texture scaled to CellSize with adjusted opacity
            spriteBatch.Draw(
                Texture,
                destinationRectangle,
                null,
                cellColor * opacity,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Depth
            );
        }
    }
}
