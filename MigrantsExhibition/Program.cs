// Program.cs
using MigrantsExhibition.Src;
using System;

namespace MigrantsExhibition
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new Game1())
                    game.Run();
            }
            catch (Exception ex)
            {
                Utils.LogError($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
                Console.WriteLine($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
