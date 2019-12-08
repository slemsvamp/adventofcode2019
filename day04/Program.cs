﻿using System;

namespace day04
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "264793-803935";

            Console.WriteLine("---- Part 01 ----");
            var part1 = FirstStar.Run(input);
            Console.WriteLine($"Result: {part1}");

            Console.WriteLine("---- Part 02 ----");
            var part2 = SecondStar.Run(input);
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
