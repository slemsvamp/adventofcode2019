using System;

namespace day15
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetBufferSize(100, 80);
            Console.SetWindowSize(100, 80);

            Console.WriteLine("---- Part 01 ----");
            var part1 = "No run"; //FirstStar.Run();
            Console.WriteLine($"Result: {part1}");
            Console.WriteLine("-----------------");

            
            Console.WriteLine("---- Part 02 ----");
            var part2 = SecondStar.Run();
            Console.WriteLine($"Result: {part2}");

            Console.WriteLine("-----------------");
            Console.WriteLine("1) Copy Part01 to Clipboard");
            Console.WriteLine("2) Copy Part02 to Clipboard");
            Console.WriteLine("Any) Quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1)
            {
                WindowsClipboard.SetText(part1);
            }
            else if (key.Key == ConsoleKey.D2)
            {
                WindowsClipboard.SetText(part2);
            }
        }
    }
}
