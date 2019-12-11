﻿using System;

namespace day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedInput = InputParser.Parse("input.txt");

            Console.WriteLine("---- Part 01 ----");
            var part1 = FirstStar.Run(parsedInput);
            Console.WriteLine($"Result: {part1}");

            Console.WriteLine("---- Part 02 ----");
            SecondStar.Run(parsedInput);
            Console.WriteLine();

            Console.WriteLine("-----------------");
            Console.WriteLine("1) Copy Part01 to Clipboard");
            Console.WriteLine("2) Copy Part02 to Clipboard");
            Console.WriteLine("Any) Quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D1)
            {
                WindowsClipboard.SetText(part1);
            }
        }
    }
}
