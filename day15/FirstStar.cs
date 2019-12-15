using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace day15
{
    public class FirstStar
    {
        private static Randomizer _randomizer;
        private static Direction _lastDirection;

        public enum Direction
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        public enum Status
        {
            Waiting = -1,
            HitAWall = 0,
            MovedOneStep = 1,
            MovedOneStepFoundOxygenSystem = 2
        }

        public static string Run()
        {
            var opcodes = InputParser.Parse("input.txt");
            var opcodeRunner = new OpcodeRunner(opcodes.Count, opcodes, GetMovementCommand);

            _randomizer = new Randomizer(seed: 987654321); //123456789

            var status = Status.Waiting;
            var grid = new Grid(new Size { Width = 40, Height = 44 });
            var position = new Point(20, 22);

            while (status != Status.MovedOneStepFoundOxygenSystem)
            {
                status = (Status)opcodeRunner.Run();

                switch (status)
                {
                    case Status.HitAWall:
                    {
                        // put a wall at position + _lastDirection
                        grid.SetPoint(
                            new Point(position.X + (_lastDirection == Direction.West ? -1 : _lastDirection == Direction.East ? 1 : 0), position.Y + (_lastDirection == Direction.North ? -1 : _lastDirection == Direction.South ? 1 : 0)),
                            GridData.Wall
                        );
                    }
                    break;
                    case Status.MovedOneStep:
                    {
                        // move to position + _lastDirection
                        position = new Point(position.X + (_lastDirection == Direction.West ? -1 : _lastDirection == Direction.East ? 1 : 0), position.Y + (_lastDirection == Direction.North ? -1 : _lastDirection == Direction.South ? 1 : 0));
                    }
                    break;
                    case Status.MovedOneStepFoundOxygenSystem:
                    {
                        // put an oxygen system at position + _lastDirection
                        grid.SetPoint(
                            new Point(position.X + (_lastDirection == Direction.West ? -1 : _lastDirection == Direction.East ? 1 : 0), position.Y + (_lastDirection == Direction.North ? -1 : _lastDirection == Direction.South ? 1 : 0)),
                            GridData.OxygenSystem
                        );
                    }
                    break;
                }
            }

            for (int y = 0; y < grid.Size.Height; y++)
            {
                for (int x = 0; x < grid.Size.Width; x++)
                {
                    Console.SetCursorPosition(x, y);
                    var data = grid.Map[y * grid.Size.Width + x];

                    if (data == (byte)GridData.Wall)
                    {
                        Console.Write("#");
                    }
                    else if (data == (byte)GridData.OxygenSystem)
                    {
                        Console.Write("O");
                    }
                }
            }

            Console.SetCursorPosition(20, 22);
            Console.Write("☺");

            // I did this manually :D (drew it out and used my human calculator, instead of using AStarpathing
            return "248";
        }

        public static long GetMovementCommand()
        {
            var movementOrder = _randomizer.GetNext(1, 4);
            _lastDirection = (Direction)movementOrder;
            return movementOrder;
        }
    }
}
