// Src/Layer.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MigrantsExhibition.Src
{
    public class Layer
    {
        public int LayerNumber { get; private set; }
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        public float CellSize { get; private set; }
        public float Depth { get; private set; }

        private Cell[,] grid;     // Current generation grid
        private Cell[,] nextGrid; // Next generation grid
        private List<Texture2D> cellTextures;
        private GraphicsDevice graphicsDevice;
        private Random random;

        public Layer(int layerNumber, float cellSize, float depth, List<Texture2D> cellTextures, GraphicsDevice graphicsDevice)
        {
            this.LayerNumber = layerNumber;
            this.CellSize = cellSize;
            this.Depth = depth;
            this.cellTextures = cellTextures;
            this.graphicsDevice = graphicsDevice;
            this.random = new Random();

            InitializeGrid();
            AdjustCellPopulation(50f); // Initialize with 50% sound intensity
        }

        private void InitializeGrid()
        {
            // Determine grid size based on cell size and viewport dimensions
            GridWidth = graphicsDevice.Viewport.Width / (int)CellSize;
            GridHeight = graphicsDevice.Viewport.Height / (int)CellSize;

            grid = new Cell[GridWidth, GridHeight];
            nextGrid = new Cell[GridWidth, GridHeight];
        }

        public void AdjustCellPopulation(float soundIntensity)
        {
            int totalCells = GridWidth * GridHeight;
            int desiredLiveCells = (int)(totalCells * (soundIntensity / 100f));

            // Collect positions of live and dead cells
            List<(int x, int y)> liveCellsPositions = new List<(int x, int y)>();
            List<(int x, int y)> deadCellsPositions = new List<(int x, int y)>();

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
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

                    // Select a random texture
                    Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                    // Calculate position based on grid coordinates
                    Vector2 position = new Vector2(
                        pos.x * CellSize + CellSize / 2,
                        pos.y * CellSize + CellSize / 2
                    );

                    // Create and place the new cell
                    Cell cell = new Cell(texture, position, Vector2.Zero, LayerNumber, graphicsDevice, depth: Depth);
                    grid[pos.x, pos.y] = cell;

                    deadCellsPositions.RemoveAt(index);
                }
            }
        }

        public void ApplyRules()
        {
            // Clear nextGrid
            Array.Clear(nextGrid, 0, nextGrid.Length);

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
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

                            // Select a random texture
                            Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                            // Calculate position based on grid coordinates
                            Vector2 position = new Vector2(
                                x * CellSize + CellSize / 2,
                                y * CellSize + CellSize / 2
                            );

                            // Create and place the new cell
                            Cell cell = new Cell(texture, position, Vector2.Zero, LayerNumber, graphicsDevice, depth: Depth);
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
            int width = GridWidth;
            int height = GridHeight;

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

        public void Update(GameTime gameTime, float soundIntensity)
        {
            // Update cells
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    grid[x, y]?.Update(gameTime, soundIntensity);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Collect all live cells
            List<Cell> allCells = new List<Cell>();

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        allCells.Add(grid[x, y]);
                    }
                }
            }

            // No need to sort cells within a layer since they have the same depth
            foreach (var cell in allCells)
            {
                cell.Draw(spriteBatch);
            }
        }
    }
}
