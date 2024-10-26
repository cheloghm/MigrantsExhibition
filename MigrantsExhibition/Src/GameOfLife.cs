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
        private List<Cell> cells;
        private Dictionary<(int, int), int> neighborCounts;

        private int initialCellCount;
        private const float CellSizeLayer1 = Constants.CellSizeLayer1; // Use centralized cell size
        private const float CellSizeLayer2 = Constants.CellSizeLayer2;

        private List<Texture2D> cellTextures;

        private Random random; // Private Random instance

        private int generationCount = 0;

        public GameOfLife(GraphicsDevice graphicsDevice, List<Texture2D> cellTextures, int initialCellCount = 50)
        {
            if (cellTextures == null || cellTextures.Count == 0)
            {
                throw new ArgumentException("cellTextures cannot be null or empty.");
            }

            this.graphicsDevice = graphicsDevice;
            this.cellTextures = cellTextures;
            this.cells = new List<Cell>();
            this.neighborCounts = new Dictionary<(int, int), int>();
            this.initialCellCount = initialCellCount;
            this.random = new Random();
            Utils.LogInfo("GameOfLife constructor executed successfully.");
        }

        public void Initialize(List<Cell> initialCells)
        {
            if (initialCells == null)
            {
                throw new ArgumentNullException(nameof(initialCells));
            }

            cells = initialCells;
            Utils.LogInfo("GameOfLife cells set during initialization.");
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Update cell states based on rules
            ApplyRules(soundIntensity);

            // Increment generation count
            generationCount++;

            // Check if after 3 generations, live cells <=15%, then generate new cells
            if (generationCount >= 3)
            {
                float totalPossibleCellsLayer1 = (graphicsDevice.Viewport.Width / CellSizeLayer1) * (graphicsDevice.Viewport.Height / CellSizeLayer1);
                float totalPossibleCellsLayer2 = (graphicsDevice.Viewport.Width / CellSizeLayer2) * (graphicsDevice.Viewport.Height / CellSizeLayer2);
                float totalPossibleCells = totalPossibleCellsLayer1 + totalPossibleCellsLayer2;

                float currentLiveCellsPercentage = (cells.Count / totalPossibleCells) * 100f;

                if (currentLiveCellsPercentage <= Constants.MinLiveCellsPercentage)
                {
                    GenerateRandomCells(soundIntensity);
                    generationCount = 0; // Reset generation count after replenishment
                }
            }
        }

        private void ApplyRules(float soundIntensity)
        {
            neighborCounts.Clear();

            // Count neighbors
            foreach (var cell in cells)
            {
                int x = (int)(cell.Position.X / GetLayerCellSize(cell.Layer)); // Grid position
                int y = (int)(cell.Position.Y / GetLayerCellSize(cell.Layer));

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue; // Skip the cell itself

                        int nx = x + dx;
                        int ny = y + dy;

                        // Wrap around the screen edges
                        int gridWidth = (int)(graphicsDevice.Viewport.Width / GetLayerCellSize(cell.Layer));
                        int gridHeight = (int)(graphicsDevice.Viewport.Height / GetLayerCellSize(cell.Layer));

                        nx = (nx + gridWidth) % gridWidth;
                        ny = (ny + gridHeight) % gridHeight;

                        var key = (nx, ny);
                        if (neighborCounts.ContainsKey(key))
                            neighborCounts[key]++;
                        else
                            neighborCounts[key] = 1;
                    }
                }
            }

            // Determine which cells survive, die, or are born
            List<Cell> survivingCells = new List<Cell>();
            List<(int, int)> cellsToDie = new List<(int, int)>();

            foreach (var cell in cells)
            {
                int x = (int)(cell.Position.X / GetLayerCellSize(cell.Layer));
                int y = (int)(cell.Position.Y / GetLayerCellSize(cell.Layer));
                var key = (x, y);

                if (neighborCounts.ContainsKey(key))
                {
                    int count = neighborCounts[key];
                    if (count == 2 || count == 3)
                    {
                        survivingCells.Add(cell); // Cell survives
                    }
                    else
                    {
                        cellsToDie.Add(key); // Cell dies
                        cell.Die(); // Trigger zoom-out animation
                        Utils.LogInfo($"Cell at ({cell.Position.X}, {cell.Position.Y}) marked to die.");
                    }
                }
                else
                {
                    cellsToDie.Add(key);
                    cell.Die();
                    Utils.LogInfo($"Cell at ({cell.Position.X}, {cell.Position.Y}) marked to die.");
                }
            }

            // Birth of new cells based on sound intensity
            List<Cell> bornCells = new List<Cell>();
            double birthProbability = GetBirthProbability(soundIntensity);

            foreach (var kvp in neighborCounts)
            {
                var position = kvp.Key;
                int count = kvp.Value;

                if (count == 3)
                {
                    // Check if cell already exists at this position
                    bool exists = cells.Exists(c =>
                        (int)(c.Position.X / GetLayerCellSize(c.Layer)) == position.Item1 &&
                        (int)(c.Position.Y / GetLayerCellSize(c.Layer)) == position.Item2);
                    if (!exists)
                    {
                        // Decide whether to spawn a new cell based on birth probability
                        if (random.NextDouble() <= birthProbability)
                        {
                            // Random layer assignment
                            int layer = random.Next(1, Constants.TotalLayers + 1);

                            // Select a random texture
                            Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                            // Spawn a new cell at this grid position
                            float layerCellSize = GetLayerCellSize(layer);
                            Vector2 newPosition = new Vector2(position.Item1 * layerCellSize + layerCellSize / 2, position.Item2 * layerCellSize + layerCellSize / 2); // Center of the grid cell
                            Vector2 direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                            direction.Normalize();

                            // Assign depth based on layer
                            float depth = layer switch
                            {
                                1 => 0.3f,
                                2 => 0.6f,
                                3 => 0.9f,
                                _ => 0.5f,
                            };

                            // Create and add the new cell
                            Cell newCell = new Cell(texture, newPosition, direction, layer, graphicsDevice, depth);
                            newCell.IsBorn = true; // Trigger zoom-in animation
                            bornCells.Add(newCell);
                            Utils.LogInfo($"Born new cell at position ({newPosition.X}, {newPosition.Y}) in Layer {layer} with Depth {depth}.");
                        }
                    }
                }
            }

            // Combine surviving cells and born cells
            List<Cell> updatedCells = new List<Cell>();
            updatedCells.AddRange(survivingCells);
            updatedCells.AddRange(bornCells);

            cells = updatedCells;
            Utils.LogInfo($"GameOfLife updated: {cells.Count} cells present.");
        }

        private float GetLayerCellSize(int layer)
        {
            return layer switch
            {
                1 => Constants.CellSizeLayer1,
                2 => Constants.CellSizeLayer2,
                3 => Constants.CellSizeLayer2 * 0.9f, // Adjust as needed
                _ => Constants.CellSizeLayer1,
            };
        }

        private double GetBirthProbability(float soundIntensity)
        {
            // Map sound intensity from 1-100 to 0-1
            float normalizedSound = MathHelper.Clamp(soundIntensity / 100f, 0f, 1f);

            // Higher sound intensity increases birth probability
            // Example: At 20% sound, base probability; increases by 7% per 10% sound above 20%
            if (normalizedSound < (Constants.SoundThresholdMedium / 100f))
                return 0.0; // No additional births

            double additionalProbability = ((normalizedSound * 100f - Constants.SoundThresholdMedium) / 10f) * Constants.GenerationIntervalIncreasePer10Sound;
            return Math.Min(additionalProbability, 0.35); // Cap at 35% probability
        }

        private void GenerateRandomCells(float soundIntensity)
        {
            int cellsToAdd = (int)((graphicsDevice.Viewport.Width * graphicsDevice.Viewport.Height) / (Constants.CellSizeLayer1 * Constants.CellSizeLayer1) * 0.15f); // 15% of total possible cells

            for (int i = 0; i < cellsToAdd; i++)
            {
                // Random layer assignment
                int layer = random.Next(1, Constants.TotalLayers + 1);

                // Select a random texture
                Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                // Random position within the viewport
                Vector2 position = new Vector2(
                    random.Next(0, graphicsDevice.Viewport.Width),
                    random.Next(0, graphicsDevice.Viewport.Height)
                );

                // Random direction (not used for movement)
                Vector2 direction = new Vector2(
                    (float)random.NextDouble() * 2 - 1,
                    (float)random.NextDouble() * 2 - 1
                );
                direction.Normalize();

                // Assign depth based on layer
                float depth = layer switch
                {
                    1 => 0.3f,
                    2 => 0.6f,
                    3 => 0.9f,
                    _ => 0.5f,
                };

                // Create and add the new cell
                Cell newCell = new Cell(texture, position, direction, layer, graphicsDevice, depth);
                newCell.IsBorn = true; // Trigger zoom-in animation
                cells.Add(newCell);
                Utils.LogInfo($"Added random cell at position ({position.X}, {position.Y}) in Layer {layer} with Depth {depth}.");
            }

            Utils.LogInfo($"Generated {cellsToAdd} random cells to maintain population.");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Sort cells by Depth (ascending)
            var sortedCells = cells.OrderBy(c => c.Depth).ToList();

            foreach (var cell in sortedCells)
            {
                cell.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            // Dispose of any unmanaged resources if necessary
        }
    }
}
