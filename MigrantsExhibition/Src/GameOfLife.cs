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
        private const float CellSizeLayer1 = Constants.CellSizeLayer1;
        private const float CellSizeLayer2 = Constants.CellSizeLayer2;
        private const float CellSizeLayer3 = Constants.CellSizeLayer3;

        private List<Texture2D> cellTextures;

        private Random random;

        private float generationTimer = 0f;
        private const float MaxGenerationInterval = 2f; // Maximum interval in seconds
        private const float MinGenerationInterval = 0.1f; // Minimum interval in seconds

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
            // Update the generation timer
            generationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate the generation interval based on sound intensity
            float generationInterval = MaxGenerationInterval - (soundIntensity / 100f) * (MaxGenerationInterval - MinGenerationInterval);

            if (generationTimer >= generationInterval)
            {
                // Apply Game of Life rules
                ApplyRules();

                // Adjust the number of live cells to match the sound intensity percentage
                AdjustLiveCellsToMatchSoundIntensity(soundIntensity);

                // Reset the timer
                generationTimer = 0f;
            }

            // Update cells
            foreach (var cell in cells)
            {
                cell.Update(gameTime, soundIntensity);
            }
        }

        private void AdjustLiveCellsToMatchSoundIntensity(float soundIntensity)
        {
            // Calculate total screen area
            float totalScreenArea = graphicsDevice.Viewport.Width * graphicsDevice.Viewport.Height;

            // Calculate desired cell area based on sound intensity
            float desiredCellArea = totalScreenArea * (soundIntensity / 100f);

            // Calculate average cell area (considering different layers)
            float averageCellArea = (CellSizeLayer1 * CellSizeLayer1 + CellSizeLayer2 * CellSizeLayer2 + CellSizeLayer3 * CellSizeLayer3) / 3f;

            // Calculate desired number of live cells
            int desiredLiveCells = (int)(desiredCellArea / averageCellArea);

            int currentLiveCells = cells.Count;

            if (currentLiveCells > desiredLiveCells)
            {
                // Need to remove cells
                int cellsToRemove = currentLiveCells - desiredLiveCells;
                RemoveRandomCells(cellsToRemove);
            }
            else if (currentLiveCells < desiredLiveCells)
            {
                // Need to add cells
                int cellsToAdd = desiredLiveCells - currentLiveCells;
                GenerateRandomCells(cellsToAdd);
            }
        }

        private void RemoveRandomCells(int cellsToRemove)
        {
            cellsToRemove = Math.Min(cellsToRemove, cells.Count);

            for (int i = 0; i < cellsToRemove; i++)
            {
                int index = random.Next(cells.Count);
                cells.RemoveAt(index);
            }
        }

        private void GenerateRandomCells(int cellsToAdd)
        {
            for (int i = 0; i < cellsToAdd; i++)
            {
                // Random layer assignment (1, 2, or 3)
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
            }
        }

        private void ApplyRules()
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

            // Birth of new cells according to Game of Life rules
            foreach (var kvp in neighborCounts)
            {
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
                    newCell.IsBorn = true; // Trigger zoom-in animation
                    newCells.Add(newCell);
                    positions.Add(kvp.Key);
                }
            }

            cells = newCells;
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
