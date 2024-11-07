// Src/GameOfLife.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MigrantsExhibition.Src
{
    public class GameOfLife : IDisposable
    {
        private GraphicsDevice graphicsDevice;
        private List<Texture2D> cellTextures;
        private List<Layer> layers;
        private float generationTimer = 0f;
        private const float GenerationInterval = 0.1f; // Fixed interval in seconds

        public GameOfLife(GraphicsDevice graphicsDevice, List<Texture2D> cellTextures)
        {
            if (cellTextures == null || cellTextures.Count == 0)
            {
                throw new ArgumentException("cellTextures cannot be null or empty.");
            }

            this.graphicsDevice = graphicsDevice;
            this.cellTextures = cellTextures;

            InitializeLayers();
            Utils.LogInfo("GameOfLife initialized successfully with multiple layers.");
        }

        private void InitializeLayers()
        {
            layers = new List<Layer>();

            // Create layers with different properties
            layers.Add(new Layer(
                layerNumber: 1,
                cellSize: Constants.CellSizeLayer1,
                depth: 0.3f,
                cellTextures: cellTextures,
                graphicsDevice: graphicsDevice));

            layers.Add(new Layer(
                layerNumber: 2,
                cellSize: Constants.CellSizeLayer2,
                depth: 0.6f,
                cellTextures: cellTextures,
                graphicsDevice: graphicsDevice));

            layers.Add(new Layer(
                layerNumber: 3,
                cellSize: Constants.CellSizeLayer3,
                depth: 0.9f,
                cellTextures: cellTextures,
                graphicsDevice: graphicsDevice));

            // For each layer, initialize cells based on initial sound intensity
            float initialSoundIntensity = 50f; // Or any other initial value

            foreach (var layer in layers)
            {
                layer.AdjustCellPopulation(initialSoundIntensity);
            }
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            generationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (generationTimer >= GenerationInterval)
            {
                foreach (var layer in layers)
                {
                    // Adjust the cell population before applying the Game of Life rules
                    layer.AdjustCellPopulation(soundIntensity);

                    // Apply Game of Life rules
                    layer.ApplyRules();
                }

                // Reset the timer
                generationTimer = 0f;
            }

            // Update cells in each layer
            foreach (var layer in layers)
            {
                layer.Update(gameTime, soundIntensity);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw layers in order of depth
            // Assuming that a lower depth value means closer to the foreground
            var sortedLayers = layers.OrderBy(layer => layer.Depth).ToList();

            foreach (var layer in sortedLayers)
            {
                layer.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            // Dispose of any unmanaged resources if necessary
        }
    }
}
