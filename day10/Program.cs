using System;

namespace day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedInput = InputParser.Parse("input.txt");

            Console.WriteLine("---- Part 01 ----");
            var part1 = FirstStar.Run(parsedInput);
            Console.WriteLine($"Result: X{part1.Coordinate.X}/Y{part1.Coordinate.Y}, Count={part1.Count}");

            Console.WriteLine("---- Part 02 ----");
            var part2 = SecondStar.Run(part1.Coordinate, parsedInput);
            Console.WriteLine($"Result: {part2}");

            Console.WriteLine("-----------------");
            Console.WriteLine("1) Copy Part01 to Clipboard");
            Console.WriteLine("2) Copy Part02 to Clipboard");
            Console.WriteLine("Any) Quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1)
            {
                WindowsClipboard.SetText($"X{part1.Coordinate.X}/Y{part1.Coordinate.Y}, Count={part1.Count}");
            }
            else if (key.Key == ConsoleKey.D2)
            {
                WindowsClipboard.SetText(part2.ToString());
            }
        }
    }
}
