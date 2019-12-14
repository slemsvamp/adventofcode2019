using System;

namespace day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var firstStarTestsSuccess = RunFirstStarTests();

            if (!firstStarTestsSuccess)
            {
                Console.WriteLine("Tests failed. Aborted.");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("---- Part 01 ----");
            var part1 = FirstStar.Run();
            Console.WriteLine($"Result: {part1}");
            Console.WriteLine("-----------------");

            var secondStarTestsSuccess = RunSecondStarTests();

            if (!secondStarTestsSuccess)
            {
                Console.WriteLine("Tests failed. Aborted.");
                Console.ReadKey(true);
                return;
            }

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

        private static bool RunFirstStarTests()
        {
            (string file, int expected)[] firstStarTestCases = new[]
            {
                ("mockInput.txt", 31),
                ("mockInput2.txt", 165),
                ("mockInput3.txt", 13312),
                ("mockInput4.txt", 180697),
                ("mockInput5.txt", 2210736)
            };

            bool failed = false;

            foreach (var firstStarTestCase in firstStarTestCases)
            {
                var chemCooker = new ChemCooker(firstStarTestCase.file);
                var result = chemCooker.OrePerFuel(1);

                if (result == firstStarTestCase.expected)
                {
                    Console.WriteLine(firstStarTestCase.file + ": Succeeded!");
                }
                else
                {
                    Console.WriteLine(firstStarTestCase.file + ": Failed, got " + result + " expected " + firstStarTestCase.expected);
                    failed = true;
                }
            }

            return !failed;
        }

        private static bool RunSecondStarTests()
        {
            (string file, int expected)[] secondStarTestCases = new[]
            {
                //("mockInput3.txt", 82892753),
                //("mockInput4.txt", 5586022),
                ("mockInput5.txt", 460664)
            };

            bool failed = false;

            foreach (var secondStarTestCase in secondStarTestCases)
            {
                var chemCooker = new ChemCooker(secondStarTestCase.file);
                var result = chemCooker.FuelForOre(1_000_000_000_000);

                if (result == secondStarTestCase.expected)
                {
                    Console.WriteLine(secondStarTestCase.file + ": Succeeded!");
                }
                else
                {
                    Console.WriteLine(secondStarTestCase.file + ": Failed, got " + result + " expected " + secondStarTestCase.expected);
                    failed = true;
                }
            }

            return !failed;
        }
    }
}
