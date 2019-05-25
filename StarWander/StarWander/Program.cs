using System;

namespace StarWander
{
    internal class Program
    {
        public static double StartTime;

        public static double CurrentTime => TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - StartTime;

        private static void Main(string[] args)
        {
            StartTime = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;

            var mode =
#if DEBUG
                args.Length == 0 ?          // No args?
                    GameWindowMode.Game :
                args[0] == "worldeditor" ?  // worldeditor?
                    GameWindowMode.WorldEditor :
                args[0] == "tileeditor" ?   // tileeditor?
                    throw new NotImplementedException() :
                GameWindowMode.Game;  // default
#else
                // Don't allow other game modes if not debugging
                GameWindowMode.Game;
#endif
            Console.WriteLine($"Starting game in {mode} mode");
            var window = new SGameWindow(mode);
            window.Run();
        }
    }
}
