// Src/ImageLoader.cs
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MigrantsExhibition.Src
{
    public class ImageLoader : IDisposable
    {
        private GraphicsDevice graphicsDevice;
        private string imagesDirectoryPath;

        public ImageLoader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            // Define the relative path to the Images directory
            imagesDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Images");
            Utils.LogInfo($"ImageLoader initialized with Images directory: {imagesDirectoryPath}");
        }

        public List<Texture2D> LoadImages()
        {
            List<Texture2D> textures = new List<Texture2D>();

            if (!Directory.Exists(imagesDirectoryPath))
            {
                Utils.LogError($"Images directory does not exist: {imagesDirectoryPath}");
                return textures;
            }

            try
            {
                // Supported image extensions
                string[] supportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

                // Get all image files in the directory
                string[] imageFiles = Directory.GetFiles(imagesDirectoryPath, "*.*", SearchOption.TopDirectoryOnly)
                                              .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                              .ToArray();

                Utils.LogInfo($"Found {imageFiles.Length} image files.");

                foreach (var imagePath in imageFiles)
                {
                    string fileName = Path.GetFileName(imagePath);
                    try
                    {
                        using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                        {
                            Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);
                            textures.Add(texture);
                            Utils.LogInfo($"Loaded image: {fileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.LogError($"Failed to load image '{fileName}': {ex.Message}");
                    }
                }

                Utils.LogInfo($"Successfully loaded {textures.Count} textures.");
            }
            catch (Exception ex)
            {
                Utils.LogError($"Error during image loading: {ex.Message}");
            }

            return textures;
        }

        public void Dispose()
        {
            // Dispose of any unmanaged resources if necessary
        }
    }
}
