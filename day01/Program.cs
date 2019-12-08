using System;

namespace day01
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var numbers = InputParser.Parse("input.txt");

            var part1 = FirstStar.Run(numbers);
            Console.WriteLine($"Part 01 - Result: {part1}");

            var part2 = SecondStar.Run(numbers);
            Console.WriteLine($"Part 02 - Result: {part2}");

            Console.WriteLine("-----------------");
            Console.WriteLine($"1) Copy {part1} to Clipboard");
            Console.WriteLine($"2) Copy {part2} to Clipboard");
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
