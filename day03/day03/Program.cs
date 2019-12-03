using System;

namespace day03
{
    class Program
    {
        static void Main(string[] args)
        {
            // 359 too high

            var parsedInput = InputParser.Parse("input.txt");

            Console.WriteLine("---- Part 01 ----");
            var part1 = new Part01().Run(parsedInput);
            Console.WriteLine($"Result: {part1.Distance.ToString()}");

            Console.WriteLine("---- Part 02 ----");
            var part2 = new Part02().Run(parsedInput, part1.Crossings);
            Console.WriteLine($"Result: {part2}");

            Console.WriteLine("-----------------");
            Console.WriteLine("1) Copy Part01 to Clipboard");
            Console.WriteLine("2) Copy Part02 to Clipboard");
            Console.WriteLine("Any) Quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1)
            {
                WindowsClipboard.SetText(part1.Distance.ToString());
            }
            else if (key.Key == ConsoleKey.D2)
            {
                WindowsClipboard.SetText(part2);
            }
        }
    }
}
