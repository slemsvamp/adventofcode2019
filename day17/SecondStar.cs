using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace day17
{
    public class SecondStar
    {
        public enum Direction
        {
            Up, Down, Left, Right
        }

        public class Robot
        {
            public Direction Facing { get; set; }
            public Prompt CurrentPrompt { get; set; }

            private CharEnumerator Routine { get; set; }
            public bool RoutineEnded { get; set; }

            private CharEnumerator Functions { get; set; }
            public bool FunctionsEnded { get; set; }

            private CharEnumerator ContinousFeed { get; set; }

            public Robot()
            {
                RoutineEnded = false;
                Routine = ("A,B,A,B,A,C,B,C,A,C" + (char)10).GetEnumerator();

                // (R4 L10 L10) (L8 R12 R10 R4) (R4 L10 L10) (L8 R12 R10 R4) (R4 L10 L10) (L8 L8 R10 R4) (L8 R12 R10 R4) (L8 L8 R10 R4) (R4 L10 L10) (L8 L8 R10 R4)
                // A B A B A C B C A C

                FunctionsEnded = false;
                Functions = ("R,4,L,10,L,10" + (char)10 +
                    "L,8,R,12,R,10,R,4" + (char)10 +
                    "L,8,L,8,R,10,R,4" + (char)10).GetEnumerator();

                ContinousFeed = ("n" + (char)10).GetEnumerator();
            }

            public long NextInRoutine()
            {
                if (!Routine.MoveNext())
                {
                    RoutineEnded = true;
                    return -1;
                }
                return Routine.Current;
            }

            public long NextInFunctions()
            {
                if (!Functions.MoveNext())
                {
                    FunctionsEnded = true;
                    return -1;
                }
                return Functions.Current;
            }

            public long NextInContinousFeed()
            {
                ContinousFeed.MoveNext();
                return ContinousFeed.Current;
            }
        }

        public enum Prompt : int
        {
            MovementRoutine = 0,
            MovementFunction = 1,
            ContinousFeed = 2
        }

        private static Robot _robot { get; set; }

        public static string Run()
        {
            _robot = new Robot();

            var opcodes = InputParser.Parse("input.txt");
            var opcodeRunner = new OpcodeRunner(4000, opcodes, AskForInput);

            RunRobot(opcodeRunner, false);

            opcodes[0] = 2;
            opcodeRunner.Halted = false;

            var result = RunRobot(opcodeRunner, false);

            return result.ToString();
        }

        private static long RunRobot(OpcodeRunner opcodeRunner, bool draw, bool special = false)
        {
            bool widthUnknown = true;

            int y = 0;
            int x = 0;

            var tileBag = new List<string>();
            int width = 39;

            while (!opcodeRunner.Halted)
            {
                var result = opcodeRunner.Run();

                if (result > 255)
                {
                    return result;
                }

                var data = (byte)result;

                if (data == 10)
                {
                    if (widthUnknown)
                    {
                        //width = x;
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
            
            var height = special ? 43 : tileBag.Count / width;
            var intersections = new List<Point>();

            back:

            for (int yLocation = 0; yLocation < height; yLocation++)
            {
                for (int xLocation = 0; xLocation < width; xLocation++)
                {
                    bool intersection = false;
                    var tile = tileBag[yLocation * width + xLocation];

                    switch (tile)
                    {
                        case ".": break;
                        case "#":
                        {
                            intersection = CheckForIntersection(tileBag, width, height, xLocation, yLocation);

                            if (intersection)
                            {
                                intersections.Add(new Point(xLocation, yLocation));
                            }
                        }
                        break;
                        case "^":
                        {
                            _robot.Facing = Direction.Up;
                        }
                        break;
                        case "v":
                        {
                            _robot.Facing = Direction.Down;
                        }
                        break;
                        case "<":
                        {
                            _robot.Facing = Direction.Left;
                        }
                        break;
                        case ">":
                        {
                            _robot.Facing = Direction.Right;
                        }
                        break;
                    }

                    if (draw)
                    {
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
            }

            if (special && tileBag.Count > width * height)
            {
                tileBag = tileBag.Skip(width * height).ToList();
                Thread.Sleep(30);
                goto back;
            }

            return -1;
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
            if (_robot.CurrentPrompt == Prompt.MovementRoutine)
            {
                var r = _robot.NextInRoutine();
                if (_robot.RoutineEnded)
                {
                    _robot.CurrentPrompt = Prompt.MovementFunction;
                }
                else
                {
                    return r;
                }
            }

            if (_robot.CurrentPrompt == Prompt.MovementFunction)
            {
                var f = _robot.NextInFunctions();
                if (_robot.FunctionsEnded)
                {
                    _robot.CurrentPrompt = Prompt.ContinousFeed;
                }
                else
                {
                    return f;
                }
            }

            if (_robot.CurrentPrompt == Prompt.ContinousFeed)
            {
                return _robot.NextInContinousFeed();
            }

            return -1;
        }
    }
}
