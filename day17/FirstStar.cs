using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace day17
{
    public class FirstStar
    {
        public static string Run()
        {
            var opcodes = InputParser.Parse("input.txt");
            var opcodeRunner = new OpcodeRunner(4000, opcodes, AskForInput);

            bool widthUnknown = true;

            int y = 0;
            int x = 0;

            var tileBag = new List<string>();
            int width = 0;

            while (!opcodeRunner.Halted)
            {
                var data = (byte)opcodeRunner.Run();

                if (data == 10)
                {
                    if (widthUnknown)
                    {
                        width = x;
                        widthUnknown = false;
                    }

                    y++;
                }
                else
                {
                    tileBag.Add(Encoding.ASCII.GetString(new byte[] { data }));
                    x++;
                }
            }

            var height = tileBag.Count / width;
            var intersections = new List<Point>();

            for (int yLocation = 0; yLocation < height; yLocation++)
            {
                for (int xLocation = 0; xLocation < width; xLocation++)
                {
                    bool intersection = false;
                    var tile = tileBag[yLocation * width + xLocation];

                    switch (tile)
                    {
                        case ".":
                        {

                        }
                        break;
                        case "#":
                        {
                            intersection = CheckForIntersection(tileBag, width, height, xLocation, yLocation);

                            if (intersection)
                            {
                                intersections.Add(new Point(xLocation, yLocation));
                            }
                        }
                        break;
                        case "^": { } break;
                        case "v": { } break;
                        case "<": { } break;
                        case ">": { } break;
                    }

                    Console.SetCursorPosition(xLocation, yLocation);

                    if (intersection)
                    {
                        Console.Write("O");
                    }
                    else
                    {
                        Console.Write(tile);
                    }
                }
            }

            var sum = 0;

            foreach (var intersection in intersections)
            {
                sum += intersection.X * intersection.Y;
            }

            return sum.ToString();
        }

        private static bool CheckForIntersection(List<string> tileBag, int width, int height, int xLocation, int yLocation)
        {
            if (yLocation > 0 && yLocation < height - 1 && xLocation > 0 && xLocation < width - 1)
            {
                if (tileBag[(yLocation - 1) * width + xLocation] != "." &&
                    tileBag[(yLocation + 1) * width + xLocation] != "." &&
                    tileBag[yLocation * width + xLocation + 1] != "." &&
                    tileBag[yLocation * width + xLocation - 1] != ".")
                {
                    return true;
                }
            }
            return false;
        }

        private static long AskForInput()
        {
            return 0;
        }
    }
}
