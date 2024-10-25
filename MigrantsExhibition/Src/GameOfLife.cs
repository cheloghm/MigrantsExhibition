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
        private const float CellSize = Constants.CellSize; // Use centralized cell size

        private List<Texture2D> cellTextures;

        private Random random; // Private Random instance

        private const float NormalSoundIntensity = Constants.NormalSoundIntensity; // Define as needed
        private const float MidSoundIntensity = Constants.MidSoundIntensity;    // Define as needed

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
            // Update each cell's position and behavior
            foreach (var cell in cells)
            {
                // Clamp soundIntensity to [0.0f, 1.0f]
                float clampedSoundIntensity = MathHelper.Clamp(soundIntensity, 0.0f, 1.0f);

                // Determine if sound intensity is above normal
                bool isAboveNormal = clampedSoundIntensity >= NormalSoundIntensity;

                // Update cell with clamped sound intensity
                cell.Update(gameTime, clampedSoundIntensity, isAboveNormal, MidSoundIntensity);
            }

            // Apply Game of Life rules
            ApplyRules();

            // Ensure minimum cell population
            EnsureMinimumPopulation();

            // Remove cells that have completed their zoom-out animation
            RemoveDyingCells();
        }

        private void ApplyRules()
        {
            neighborCounts.Clear();

            // Count neighbors
            foreach (var cell in cells)
            {
                int x = (int)(cell.Position.X / CellSize); // Grid position
                int y = (int)(cell.Position.Y / CellSize);

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue; // Skip the cell itself

                        int nx = x + dx;
                        int ny = y + dy;

                        // Wrap around the screen edges
                        int gridWidth = graphicsDevice.Viewport.Width / (int)CellSize;
                        int gridHeight = graphicsDevice.Viewport.Height / (int)CellSize;

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
                int x = (int)(cell.Position.X / CellSize);
                int y = (int)(cell.Position.Y / CellSize);
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

            // Birth of new cells
            List<Cell> bornCells = new List<Cell>();
            foreach (var kvp in neighborCounts)
            {
                var position = kvp.Key;
                int count = kvp.Value;

                if (count == 3)
                {
                    // Check if cell already exists at this position
                    bool exists = cells.Exists(c => (int)(c.Position.X / CellSize) == position.Item1 && (int)(c.Position.Y / CellSize) == position.Item2);
                    if (!exists)
                    {
                        // Spawn a new cell at this grid position
                        Vector2 newPosition = new Vector2(position.Item1 * CellSize + CellSize / 2, position.Item2 * CellSize + CellSize / 2); // Center of the grid cell
                        Vector2 direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                        direction.Normalize();
                        float speed = Constants.BaseSpeed; // Use centralized speed

                        // Assign depth based on desired layering
                        float depth = AssignDepth();

                        // Ensure cellTextures is not empty
                        if (cellTextures.Count > 0)
                        {
                            Texture2D texture = cellTextures[random.Next(cellTextures.Count)];
                            Cell newCell = new Cell(texture, newPosition, direction, speed, graphicsDevice, depth);
                            bornCells.Add(newCell);
                            Utils.LogInfo($"Born new cell at position ({newPosition.X}, {newPosition.Y}) with depth {depth}.");
                        }
                        else
                        {
                            Utils.LogError("cellTextures list is empty. Cannot spawn new cells.");
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

        private float AssignDepth()
        {
            double depthRandom = random.NextDouble();
            float depth;
            if (depthRandom < 0.1)
                depth = 0.1f; // Foreground layer 1
            else if (depthRandom < 0.3)
                depth = 0.3f; // Foreground layer 2
            else if (depthRandom < 0.6)
                depth = 0.6f; // Background layer 1
            else
                depth = 0.9f; // Background layer 2
            return depth;
        }

        private void EnsureMinimumPopulation()
        {
            int minimumCells = (int)(initialCellCount * 0.2f); // 20% of initial cells
            if (cells.Count < minimumCells)
            {
                int cellsToAdd = minimumCells - cells.Count;
                Utils.LogInfo($"Cell count below minimum. Adding {cellsToAdd} cells.");

                for (int i = 0; i < cellsToAdd; i++)
                {
                    if (cellTextures.Count == 0)
                    {
                        Utils.LogError("No cell textures available to add.");
                        break;
                    }

                    // Select a random texture
                    Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                    // Random position within the viewport
                    Vector2 position = new Vector2(
                        random.Next(0, graphicsDevice.Viewport.Width),
                        random.Next(0, graphicsDevice.Viewport.Height)
                    );

                    // Random direction
                    Vector2 direction = new Vector2(
                        (float)random.NextDouble() * 2 - 1,
                        (float)random.NextDouble() * 2 - 1
                    );
                    direction.Normalize();

                    // Assign depth: 10% chance for foreground layers, 90% for background layers
                    float depth = AssignDepth();

                    // Create and add the new cell with depth
                    Cell newCell = new Cell(texture, position, direction, Constants.BaseSpeed, graphicsDevice, depth);
                    newCell.IsBorn = true; // Trigger zoom-in animation
                    cells.Add(newCell);
                    Utils.LogInfo($"Added new cell to maintain population: Position=({position.X}, {position.Y}), Depth={depth}.");
                }

                Utils.LogInfo($"Added {cellsToAdd} cells to maintain minimum population. Total cells: {cells.Count}");
            }
        }

        private void RemoveDyingCells()
        {
            List<Cell> cellsToRemove = new List<Cell>();
            foreach (var cell in cells)
            {
                if (cell.IsDying && cell.Scale <= 0.0f)
                {
                    cellsToRemove.Add(cell);
                    Utils.LogInfo($"Removing cell at ({cell.Position.X}, {cell.Position.Y}) after death animation.");
                }
            }

            foreach (var cell in cellsToRemove)
            {
                cells.Remove(cell);
            }
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
