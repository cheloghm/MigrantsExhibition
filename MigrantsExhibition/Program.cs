// Program.cs
using MigrantsExhibition.Src;
using System;
using System.Runtime.InteropServices;

namespace MigrantsExhibition
{
    public static class Program
    {
        // Import SetThreadExecutionState API
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);

        // Execution state flags
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;

        [STAThread]
        static void Main()
        {
            try
            {
                // Prevent sleep mode
                PreventSleep();

                using (var game = new Game1())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                Utils.LogError($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
                Console.WriteLine($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                // Restore sleep mode settings when the application exits
                RestoreSleep();
            }
        }

        static void PreventSleep()
        {
            // Set new execution state to prevent sleep and keep display on
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
        }

        static void RestoreSleep()
        {
            // Clear EXECUTION_STATE flags to allow the system to sleep normally
            SetThreadExecutionState(ES_CONTINUOUS);
        }
    }
}
