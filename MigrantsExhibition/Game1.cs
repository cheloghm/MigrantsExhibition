// Game1.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MigrantsExhibition.Src;
using System;
using System.Collections.Generic;

namespace MigrantsExhibition
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public List<Texture2D> cellTextures = new List<Texture2D>(); // Ensure this is public or provide access

        private GameOfLife gameOfLife;
        private GUI gui;
        private AudioHandler audioHandler;
        private ImageLoader imageLoader;
        private List<Star> stars = new List<Star>();
        private List<Cell> cells = new List<Cell>();
        private const int initialCellCount = 50;
        private const int StarCount = 100;
        private double elapsedTime = 0;
        private int frameCount = 0;
        private float fps = 0f;

        private float soundIntensity = 0f; // Declared at class level
        private double generationTimer = 0.0; // Timer for Game of Life generations

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Enable full-screen mode
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Utils.InitializeLogging(); // Initialize logging first
            Utils.LogInfo("Application started.");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            try
            {
                _spriteBatch = new SpriteBatch(GraphicsDevice);
                Utils.LogInfo("SpriteBatch initialized.");

                // Initialize AudioHandler
                audioHandler = new AudioHandler();
                if (!audioHandler.Start())
                {
                    Utils.LogError("Failed to start audio handler.");
                }
                else
                {
                    Utils.LogInfo("AudioHandler started successfully.");
                }

                // Initialize ImageLoader with GraphicsDevice
                imageLoader = new ImageLoader(GraphicsDevice);
                Utils.LogInfo("ImageLoader initialized.");

                cellTextures = imageLoader.LoadImages();
                Utils.LogInfo($"Loaded {cellTextures.Count} cell textures.");

                if (cellTextures.Count == 0)
                {
                    Utils.LogError("No cell textures loaded. Exiting application.");
                    Exit();
                    return; // Ensure the method exits
                }

                // Initialize GameOfLife with cellTextures
                gameOfLife = new GameOfLife(GraphicsDevice, cellTextures, initialCellCount);
                Utils.LogInfo("GameOfLife initialized.");

                // Initialize cells with fixed size and depth
                for (int i = 0; i < initialCellCount; i++)
                {
                    Texture2D texture = cellTextures[random.Next(cellTextures.Count)];
                    Vector2 position = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height));
                    Vector2 direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                    direction.Normalize();
                    float speed = Constants.BaseSpeed; // Use centralized speed
                    float depth = AssignDepth(); // Use method to assign depth

                    Cell newCell = new Cell(texture, position, direction, speed, GraphicsDevice, depth);
                    newCell.IsBorn = false; // Ensure initial cells don't trigger zoom-in
                    cells.Add(newCell);
                }
                Utils.LogInfo($"{initialCellCount} cells initialized.");

                // Initialize GameOfLife with cells
                gameOfLife.Initialize(cells);
                Utils.LogInfo("GameOfLife cells initialized.");

                // Initialize stars
                for (int i = 0; i < StarCount; i++)
                {
                    stars.Add(new Star(GraphicsDevice));
                }
                Utils.LogInfo($"{StarCount} stars initialized.");

                // Initialize GUI with Content Manager and GraphicsDevice
                gui = new GUI(Content, GraphicsDevice);
                Utils.LogInfo("GUI initialized.");
            }
            catch (Exception ex)
            {
                Utils.LogError($"Exception in LoadContent: {ex.Message}\n{ex.StackTrace}");
                Exit(); // Ensure the application exits gracefully
            }
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                // Handle exit on Escape key press
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (audioHandler != null)
                {
                    // Update audio handler
                    audioHandler.Update();
                    soundIntensity = audioHandler.GetSoundIntensity(); // Assign to class-level variable

                    // Update generation timer
                    generationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (generationTimer >= Constants.GenerationInterval)
                    {
                        // Update GameOfLife with sound intensity
                        if (gameOfLife != null)
                        {
                            gameOfLife.Update(gameTime, soundIntensity);
                        }

                        // Reset generation timer
                        generationTimer = 0.0;
                    }

                    // Update GUI with sound intensity
                    if (gui != null)
                    {
                        gui.Update(soundIntensity);
                    }
                }
                else
                {
                    Utils.LogError("audioHandler is null.");
                }

                // Update stars with sound intensity
                foreach (var star in stars)
                {
                    star.Update(gameTime, soundIntensity);
                }

                // Log sound intensity and FPS periodically
                elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                frameCount++;
                if (elapsedTime >= 1.0)
                {
                    fps = frameCount / (float)elapsedTime;
                    Utils.LogInfo($"Sound Intensity: {soundIntensity:F2}, FPS: {fps:F2}");
                    elapsedTime = 0;
                    frameCount = 0;
                }

                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                Utils.LogError($"Exception in Update: {ex.Message}\n{ex.StackTrace}");
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {
                GraphicsDevice.Clear(Color.Black);

                _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

                // Draw stars first (background elements)
                foreach (var star in stars)
                {
                    star.Draw(_spriteBatch);
                }

                // Draw cells
                if (gameOfLife != null)
                {
                    gameOfLife.Draw(_spriteBatch);
                }

                // Draw GUI
                if (gui != null)
                {
                    gui.Draw(_spriteBatch);
                }

                // Draw FPS
                string fpsText = $"FPS: {fps:F2}";
                _spriteBatch.DrawString(gui.Font, fpsText, new Vector2(10, 50), Color.White);

                _spriteBatch.End();

                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                Utils.LogError($"Exception in Draw: {ex.Message}\n{ex.StackTrace}");
                Exit();
            }
        }

        protected override void UnloadContent()
        {
            // Dispose of unmanaged resources
            gameOfLife?.Dispose();
            gui?.Dispose();
            imageLoader?.Dispose();
            audioHandler?.Dispose();
            Star.DisposePixel();

            base.UnloadContent();
        }

        // Static Random instance to be used across the Game1 class
        private static readonly Random random = new Random();

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
    }
}
