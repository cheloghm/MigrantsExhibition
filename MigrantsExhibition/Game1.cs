// Game1.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MigrantsExhibition.Src;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MigrantsExhibition
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameOfLife gameOfLife;
        private GUI gui;
        private AudioHandler audioHandler;
        private ImageLoader imageLoader;
        private List<Star> stars = new List<Star>();
        private List<Cell> cells = new List<Cell>();
        private const int initialCellCount = Constants.InitialCellCount;
        private const int StarCount = Constants.StarCountPerLayer;
        private double elapsedTime = 0;
        private int frameCount = 0;
        private float fps = 0f;

        private float soundIntensity = 0f; // Declared at class level

        private bool isFadingIn = true;
        private float fadeOpacity = 1.0f; // Start fully opaque

        // P/Invoke declarations for window manipulation
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

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

            // Set target frame rate
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = false; // Disable VSync for higher FPS
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Constants.TargetFPS);
        }

        protected override void Initialize()
        {
            Utils.InitializeLogging(); // Initialize logging first
            Utils.LogInfo("Application started.");
            base.Initialize();

            // Initialize Stars across three layers
            for (int i = 0; i < StarCount; i++)
            {
                stars.Add(new Star(GraphicsDevice, layer: 1)); // Farthest layer
                stars.Add(new Star(GraphicsDevice, layer: 2)); // Middle layer
                stars.Add(new Star(GraphicsDevice, layer: 3)); // Nearest layer
            }

            Utils.LogInfo($"Initialized {StarCount * 3} stars across 3 layers.");

            // Initialize keyboard states
            previousKeyboardState = Keyboard.GetState();
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

                List<Texture2D> cellTextures = imageLoader.LoadImages();
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
                InitializeCells(cellTextures);

                // Initialize GameOfLife with cells
                gameOfLife.Initialize(cells);
                Utils.LogInfo("GameOfLife cells initialized.");

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

        private void InitializeCells(List<Texture2D> cellTextures)
        {
            // Cell sizes for each layer
            float cellSizeLayer1 = Constants.CellSizeLayer1;
            float cellSizeLayer2 = Constants.CellSizeLayer2;
            float cellSizeLayer3 = Constants.CellSizeLayer3;

            // Calculate total possible cells based on screen size and cell sizes
            int totalPossibleCellsLayer1 = (int)(GraphicsDevice.Viewport.Width / cellSizeLayer1) * (int)(GraphicsDevice.Viewport.Height / cellSizeLayer1);
            int totalPossibleCellsLayer2 = (int)(GraphicsDevice.Viewport.Width / cellSizeLayer2) * (int)(GraphicsDevice.Viewport.Height / cellSizeLayer2);
            int totalPossibleCellsLayer3 = (int)(GraphicsDevice.Viewport.Width / cellSizeLayer3) * (int)(GraphicsDevice.Viewport.Height / cellSizeLayer3);

            int totalPossibleCells = totalPossibleCellsLayer1 + totalPossibleCellsLayer2 + totalPossibleCellsLayer3;

            // Ensure at least initialCellCount live cells
            int initialLiveCells = Math.Max(initialCellCount, (int)(totalPossibleCells * 0.05f)); // 5% as initial

            for (int i = 0; i < initialLiveCells; i++)
            {
                // Random layer assignment (1, 2, or 3)
                int layer = random.Next(1, Constants.TotalLayers + 1);

                // Select a random texture
                Texture2D texture = cellTextures[random.Next(cellTextures.Count)];

                // Random position within the viewport
                Vector2 position = new Vector2(
                    random.Next(0, GraphicsDevice.Viewport.Width),
                    random.Next(0, GraphicsDevice.Viewport.Height)
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
                Cell newCell = new Cell(texture, position, direction, layer, GraphicsDevice, depth);
                cells.Add(newCell);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                // Get the current keyboard state
                currentKeyboardState = Keyboard.GetState();

                // Handle exit on Escape key press
                if (currentKeyboardState.IsKeyDown(Keys.Escape))
                    Exit();

                // Toggle Full-Screen Mode on F11 Key Press
                if (IsKeyPressed(Keys.F11))
                {
                    ToggleFullScreen();
                }

                // Minimize/Restore Window on F12 Key Press
                if (IsKeyPressed(Keys.F12))
                {
                    ToggleMinimizeRestore();
                }

                // Update previous keyboard state
                previousKeyboardState = currentKeyboardState;

                if (audioHandler != null)
                {
                    // Update audio handler
                    audioHandler.Update();
                    soundIntensity = audioHandler.GetSoundIntensity(); // Assign to class-level variable

                    // Update GameOfLife with sound intensity
                    if (gameOfLife != null)
                    {
                        gameOfLife.Update(gameTime, soundIntensity);
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

                // Handle initial fade-in
                if (isFadingIn)
                {
                    fadeOpacity -= (float)(gameTime.ElapsedGameTime.TotalSeconds / Constants.FadeInDuration);
                    if (fadeOpacity <= 0f)
                    {
                        fadeOpacity = 0f;
                        isFadingIn = false;
                        Utils.LogInfo("Fade-in complete.");
                    }
                }

                // Log sound intensity and FPS periodically
                elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                frameCount++;
                if (elapsedTime >= 1.0)
                {
                    fps = frameCount / (float)elapsedTime;
                    Utils.LogInfo($"Sound Intensity: {soundIntensity:F2}%, FPS: {fps:F2}");
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

        /// <summary>
        /// Checks if a key was just pressed (not held down).
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed; otherwise, false.</returns>
        private bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Toggles the full-screen mode.
        /// </summary>
        private void ToggleFullScreen()
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
            Utils.LogInfo($"Full-Screen Mode Toggled to: {_graphics.IsFullScreen}");
        }

        /// <summary>
        /// Toggles between minimizing and restoring the window.
        /// </summary>
        private void ToggleMinimizeRestore()
        {
            var handle = Window.Handle;
            if (IsIconic(handle))
            {
                // Window is minimized; restore it
                ShowWindow(handle, SW_RESTORE);
                SetForegroundWindow(handle);
                Utils.LogInfo("Window Restored from Minimized State.");
            }
            else
            {
                // Window is not minimized; minimize it
                ShowWindow(handle, SW_MINIMIZE);
                Utils.LogInfo("Window Minimized.");
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
                _spriteBatch.DrawString(gui.Font, fpsText, new Vector2(10, 60), Color.White);

                // Draw initial fade-in overlay
                if (isFadingIn)
                {
                    _spriteBatch.Draw(
                        gui.OverlayTexture,
                        new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                        Color.Black * fadeOpacity
                    );
                }

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
    }
}
