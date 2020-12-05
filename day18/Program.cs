﻿using System;

namespace day18
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.SetWindowSize(83, 80);
            Console.SetBufferSize(83, 80);
#endif

            Console.WriteLine("---- Part 01 ----");
            var part1 = FirstStar.Run();
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
