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
        private Random random;

        private float generationTimer = 0f;
        private const float GenerationInterval = 0.1f; // Fixed interval in seconds

        private int gridWidth;
        private int gridHeight;
        private Cell[,] grid;     // Current generation grid
        private Cell[,] nextGrid; // Next generation grid

        public GameOfLife(GraphicsDevice graphicsDevice, List<Texture2D> cellTextures)
        {
            if (cellTextures == null || cellTextures.Count == 0)
            {
                throw new ArgumentException("cellTextures cannot be null or empty.");
            }

            this.graphicsDevice = graphicsDevice;
            this.cellTextures = cellTextures;
            this.random = new Random();

            InitializeGrid();
            Utils.LogInfo("GameOfLife initialized successfully.");
        }

        private void InitializeGrid()
        {
            // Determine grid size based on base cell size and viewport dimensions
            int cellSize = (int)Constants.BaseCellSize;
            gridWidth = graphicsDevice.Viewport.Width / cellSize;
            gridHeight = graphicsDevice.Viewport.Height / cellSize;

            grid = new Cell[gridWidth, gridHeight];
            nextGrid = new Cell[gridWidth, gridHeight];

            // Initialize the grid with random live cells
            InitializeCells();
        }

        private void InitializeCells()
        {
            // Assume initial sound intensity is 50%
            float initialSoundIntensity = 50f;
            AdjustCellPopulation(initialSoundIntensity);
        }

        public void Update(GameTime gameTime, float soundIntensity)
        {
            generationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (generationTimer >= GenerationInterval)
            {
                // Adjust the cell population before applying the Game of Life rules
                AdjustCellPopulation(soundIntensity);

                // Apply Game of Life rules
                ApplyRules();

                // Reset the timer
                generationTimer = 0f;
            }

            // Update cells
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y]?.Update(gameTime, soundIntensity);
                }
            }
        }

        private void AdjustCellPopulation(float soundIntensity)
        {
            int totalCells = gridWidth * gridHeight;
            int desiredLiveCells = (int)(totalCells * (soundIntensity / 100f));

            // Collect positions of live and dead cells
            List<(int x, int y)> liveCellsPositions = new List<(int x, int y)>();
            List<(int x, int y)> deadCellsPositions = new List<(int x, int y)>();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        liveCellsPositions.Add((x, y));
                    }
                    else
                    {
                        deadCellsPositions.Add((x, y));
                    }
                }
            }

            int currentLiveCells = liveCellsPositions.Count;

            if (currentLiveCells > desiredLiveCells)
            {
                // Need to kill some cells
                int cellsToKill = currentLiveCells - desiredLiveCells;
                for (int i = 0; i < cellsToKill && liveCellsPositions.Count > 0; i++)
                {
                    int index = random.Next(liveCellsPositions.Count);
                    var pos = liveCellsPositions[index];
                    grid[pos.x, pos.y] = null;
                    liveCellsPositions.RemoveAt(index);
                }
            }
            else if (currentLiveCells < desiredLiveCells)
            {
                // Need to revive some cells
                int cellsToRevive = desiredLiveCells - currentLiveCells;
                for (int i = 0; i < cellsToRevive && deadCellsPositions.Count > 0; i++)
                {
                    int index = random.Next(deadCellsPositions.Count);
                    var pos = deadCellsPositions[index];

                    // Random layer assignment
                    int layer = random.Next(1, Constants.TotalLayers + 1);

                    // Select a random texture
                    Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                    // Calculate position based on grid coordinates
                    Vector2 position = new Vector2(
                        pos.x * Constants.BaseCellSize + Constants.BaseCellSize / 2,
                        pos.y * Constants.BaseCellSize + Constants.BaseCellSize / 2
                    );

                    // Create and place the new cell
                    Cell cell = new Cell(texture, position, Vector2.Zero, layer, graphicsDevice);
                    grid[pos.x, pos.y] = cell;

                    deadCellsPositions.RemoveAt(index);
                }
            }
        }

        private void ApplyRules()
        {
            // Clear nextGrid
            Array.Clear(nextGrid, 0, nextGrid.Length);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int liveNeighbors = CountLiveNeighbors(x, y);

                    if (grid[x, y] != null)
                    {
                        // Cell is alive
                        if (liveNeighbors == 2 || liveNeighbors == 3)
                        {
                            // Cell survives
                            nextGrid[x, y] = grid[x, y];
                        }
                        else
                        {
                            // Cell dies
                            nextGrid[x, y] = null;
                        }
                    }
                    else
                    {
                        // Cell is dead
                        if (liveNeighbors == 3)
                        {
                            // Cell becomes alive
                            // Random layer assignment
                            int layer = random.Next(1, Constants.TotalLayers + 1);

                            // Select a random texture
                            Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                            // Calculate position based on grid coordinates
                            Vector2 position = new Vector2(
                                x * Constants.BaseCellSize + Constants.BaseCellSize / 2,
                                y * Constants.BaseCellSize + Constants.BaseCellSize / 2
                            );

                            // Create and place the new cell
                            Cell cell = new Cell(texture, position, Vector2.Zero, layer, graphicsDevice);
                            nextGrid[x, y] = cell;
                        }
                        else
                        {
                            // Cell remains dead
                            nextGrid[x, y] = null;
                        }
                    }
                }
            }

            // Swap grids
            var temp = grid;
            grid = nextGrid;
            nextGrid = temp;
        }

        private int CountLiveNeighbors(int x, int y)
        {
            int count = 0;
            int width = gridWidth;
            int height = gridHeight;

            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = (x + dx + width) % width;

                for (int dy = -1; dy <= 1; dy++)
                {
                    int ny = (y + dy + height) % height;

                    if (dx == 0 && dy == 0)
                        continue;

                    if (grid[nx, ny] != null)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Collect all live cells
            List<Cell> allCells = new List<Cell>();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        allCells.Add(grid[x, y]);
                    }
                }
            }

            // Sort cells by Depth (ascending)
            var sortedCells = allCells.OrderBy(c => c.Depth).ToList();

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
