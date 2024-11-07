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
        private float CellSizeLayer1 => Constants.CellSizeLayer1;
        private float CellSizeLayer2 => Constants.CellSizeLayer2;
        private float CellSizeLayer3 => Constants.CellSizeLayer3;

        private List<Texture2D> cellTextures;

        private Random random;

        public GameOfLife(GraphicsDevice graphicsDevice, List<Texture2D> cellTextures, int initialCellCount = Constants.InitialCellCount)
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
        }

        public void Initialize(List<Cell> initialCells)
        {
            if (initialCells == null)
            {
                throw new ArgumentNullException(nameof(initialCells));
            }

            cells = initialCells;
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Calculate desired number of live cells
            int desiredLiveCells = CalculateDesiredLiveCells(soundIntensity);

            if (soundIntensity >= Constants.SoundThresholdHigh)
            {
                // Stop dying and generation of cells
                // Cells will vibrate based on sound intensity
                foreach (var cell in cells)
                {
                    cell.Update(gameTime, soundIntensity);
                }
            }
            else
            {
                // Apply Game of Life rules with the desired live cells count
                ApplyRules(desiredLiveCells);

                // Update cells
                foreach (var cell in cells)
                {
                    cell.Update(gameTime, soundIntensity);
                }

                // Ensure the total number of live cells matches the desired number
                AdjustLiveCellsToMatchDesiredCount(desiredLiveCells);
            }
        }

        private int CalculateDesiredLiveCells(float soundIntensity)
        {
            float desiredLiveCellsPercentage;

            if (soundIntensity < Constants.SoundThresholdLow)
            {
                desiredLiveCellsPercentage = Constants.MinLiveCellsPercentage; // Fixed at 10%
            }
            else if (soundIntensity >= Constants.SoundThresholdHigh)
            {
                // When sound intensity is >= 75%, maintain current number of cells
                desiredLiveCellsPercentage = (cells.Count / GetTotalPossibleCells()) * 100f;
            }
            else
            {
                // The desired percentage matches the sound intensity
                desiredLiveCellsPercentage = soundIntensity;
            }

            // Calculate total possible cells
            float totalPossibleCells = GetTotalPossibleCells();

            // Calculate desired number of live cells
            int desiredLiveCells = (int)(totalPossibleCells * (desiredLiveCellsPercentage / 100f));

            // Ensure desiredLiveCells does not exceed MaxCells
            desiredLiveCells = Math.Min(desiredLiveCells, Constants.MaxCells);

            return desiredLiveCells;
        }

        private void ApplyRules(int desiredLiveCells)
        {
            neighborCounts.Clear();

            // Count neighbors
            foreach (var cell in cells)
            {
                int x = (int)(cell.Position.X / GetLayerCellSize(cell.Layer));
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

            // Determine which cells survive
            List<Cell> newCells = new List<Cell>();
            HashSet<(int, int)> positions = new HashSet<(int, int)>();

            foreach (var cell in cells)
            {
                int x = (int)(cell.Position.X / GetLayerCellSize(cell.Layer));
                int y = (int)(cell.Position.Y / GetLayerCellSize(cell.Layer));
                var key = (x, y);

                int count = neighborCounts.ContainsKey(key) ? neighborCounts[key] : 0;

                if (count == 2 || count == 3)
                {
                    // Cell survives
                    newCells.Add(cell);
                    positions.Add(key);
                }
                else
                {
                    // Cell dies
                    // Do not add to newCells
                }
            }

            // Calculate how many cells we can still add without exceeding desiredLiveCells
            int cellsToAdd = desiredLiveCells - newCells.Count;

            // Birth of new cells
            foreach (var kvp in neighborCounts)
            {
                if (cellsToAdd <= 0)
                    break;

                if (kvp.Value == 3 && !positions.Contains(kvp.Key))
                {
                    // Random layer assignment
                    int layer = random.Next(1, Constants.TotalLayers + 1);

                    // Select a random texture
                    Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                    // Spawn a new cell at this grid position
                    float layerCellSize = GetLayerCellSize(layer);
                    Vector2 newPosition = new Vector2(kvp.Key.Item1 * layerCellSize + layerCellSize / 2, kvp.Key.Item2 * layerCellSize + layerCellSize / 2);
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
                    newCells.Add(newCell);
                    positions.Add(kvp.Key);

                    cellsToAdd--;
                }
            }

            cells = newCells;
        }

        private void AdjustLiveCellsToMatchDesiredCount(int desiredLiveCells)
        {
            int currentLiveCells = cells.Count;

            if (currentLiveCells > desiredLiveCells)
            {
                // Need to remove cells
                int cellsToRemove = currentLiveCells - desiredLiveCells;
                RemoveRandomCells(cellsToRemove);
            }
        }

        private float GetTotalPossibleCells()
        {
            float totalPossibleCellsLayer1 = (graphicsDevice.Viewport.Width / CellSizeLayer1) * (graphicsDevice.Viewport.Height / CellSizeLayer1);
            float totalPossibleCellsLayer2 = (graphicsDevice.Viewport.Width / CellSizeLayer2) * (graphicsDevice.Viewport.Height / CellSizeLayer2);
            float totalPossibleCellsLayer3 = (graphicsDevice.Viewport.Width / CellSizeLayer3) * (graphicsDevice.Viewport.Height / CellSizeLayer3);

            float totalPossibleCells = totalPossibleCellsLayer1 + totalPossibleCellsLayer2 + totalPossibleCellsLayer3;

            return totalPossibleCells;
        }

        private void GenerateRandomCells(int cellsToAdd)
        {
            int maxCellsToAdd = Constants.MaxCells - cells.Count;
            cellsToAdd = Math.Min(cellsToAdd, maxCellsToAdd);

            if (cellsToAdd <= 0)
            {
                // Maximum number of cells reached. No new cells added.
                return;
            }

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
                cells.Add(newCell);
            }
        }

        private void RemoveRandomCells(int cellsToRemove)
        {
            for (int i = 0; i < cellsToRemove && cells.Count > 0; i++)
            {
                int index = random.Next(cells.Count);
                cells.RemoveAt(index);
            }
        }

        private float GetLayerCellSize(int layer)
        {
            return layer switch
            {
                1 => CellSizeLayer1,
                2 => CellSizeLayer2,
                3 => CellSizeLayer3,
                _ => CellSizeLayer1,
            };
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
