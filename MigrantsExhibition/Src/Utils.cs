// Src/Utils.cs
using System;
using System.IO;

namespace MigrantsExhibition.Src
{
    public static class Utils
    {
        private static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "application.log");

        public static void InitializeLogging()
        {
            try
            {
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                if (!File.Exists(logFilePath))
                {
                    using (File.Create(logFilePath)) { }
                }

                LogInfo("Logging initialized.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize logging: {ex.Message}");
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                string logMessage = $"INFO [{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: {message}";
                File.AppendAllText(logFilePath, $"{logMessage}{Environment.NewLine}");
                Console.WriteLine(logMessage); // Optional: Also write to console for immediate feedback
            }
            catch
            {
                // If logging fails, write to console
                Console.WriteLine($"INFO [{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: {message}");
            }
        }

        public static void LogError(string message)
        {
            try
            {
                string logMessage = $"ERROR [{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: {message}";
                File.AppendAllText(logFilePath, $"{logMessage}{Environment.NewLine}");
                Console.WriteLine(logMessage); // Optional: Also write to console for immediate feedback
            }
            catch
            {
                // If logging fails, write to console
                Console.WriteLine($"ERROR [{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: {message}");
            }
        }
    }
}
