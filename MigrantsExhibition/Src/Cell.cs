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
        public float Scale { get; set; } // Scale is modified for zoom-in/out
        public float Depth { get; private set; } // 0.0f (foreground) to 1.0f (background)
        public int Layer { get; private set; } // 1, 2, 3

        private GraphicsDevice graphicsDevice;
        private static Texture2D shadowTexture;
        private static readonly Random random = new Random();

        // Vibration offset
        private Vector2 vibrationOffset = Vector2.Zero;

        public Cell(Texture2D texture, Vector2 position, Vector2 direction, int layer, GraphicsDevice graphicsDevice, float depth = 0.5f)
        {
            Texture = texture;
            Position = position;
            Layer = layer;
            Depth = MathHelper.Clamp(depth, 0.0f, 1.0f); // Ensure depth is between 0 and 1
            this.graphicsDevice = graphicsDevice;

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
                Vibrate(soundIntensity);
            }
            else
            {
                vibrationOffset = Vector2.Zero; // Reset vibration offset
            }
        }

        private void Vibrate(float soundIntensity)
        {
            // Vibration intensity increases with sound intensity
            float excessSound = soundIntensity - Constants.SoundThresholdHigh;
            float normalizedExcessSound = excessSound / (100f - Constants.SoundThresholdHigh);
            float vibrationIntensity = Constants.CellVibrationIntensityHigh + normalizedExcessSound * (Constants.CellVibrationIntensityMax - Constants.CellVibrationIntensityHigh);

            // Apply vibration
            float vibrationAmount = vibrationIntensity;
            float offsetX = ((float)random.NextDouble() * 2 - 1) * vibrationAmount;
            float offsetY = ((float)random.NextDouble() * 2 - 1) * vibrationAmount;
            vibrationOffset = new Vector2(offsetX, offsetY);
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
                    float distanceSq = pos.LengthSquared();

                    if (distanceSq <= radiussq)
                    {
                        float alpha = 1.0f - (float)Math.Sqrt(distanceSq) / radius; // Fade out towards edges
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

            // Calculate the drawing position with vibration offset
            Vector2 drawPosition = Position + vibrationOffset;

            // Define the destination rectangle with desired size
            Rectangle destinationRectangle = new Rectangle(
                (int)(drawPosition.X - (currentCellSize * Scale) / 2),
                (int)(drawPosition.Y - (currentCellSize * Scale) / 2),
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
