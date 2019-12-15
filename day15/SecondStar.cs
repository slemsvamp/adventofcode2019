using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace day15
{
    public class SecondStar
    {
        private static Direction _lastDirection;

        [Flags]
        public enum ExploreInfo : int
        {
            Wall = 1,
            Ground = 2,
            CanWalkNorth = 4,
            CanWalkSouth = 8,
            CanWalkWest = 16,
            CanWalkEast = 32
        }

        public enum Direction
        {
            None = 0,
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

        private static bool[] _scouted;
        private static int[] _explored;

        private static int _state;

        private static OpcodeRunner _opcodeRunner { get; set; }
        private static Size _size { get; set; }
        private static Grid _grid { get; set; }
        private static Point _position { get; set; }
        public static Point _oxygenSystem { get; set; }

        private static Stack<Direction> _history;

        private class PathTree
        {
            public Stack<Direction> History { get; set; }

            public PathTree()
            {
                History = new Stack<Direction>();
            }
        }

        private static Stack<PathTree> _pathTrees { get; set; }

        public static string Run()
        {
            var opcodes = InputParser.Parse("input.txt");

            _pathTrees = new Stack<PathTree>();
            _pathTrees.Push(new PathTree());

            _opcodeRunner = new OpcodeRunner(opcodes.Count, opcodes, GetMovementCommand);
            _history = new Stack<Direction>();
            _size = new Size { Width = 40, Height = 44 };
            _grid = new Grid(_size);
            _position = new Point(20, 22);
            _oxygenSystem = new Point(0, 10);

            _scouted = new bool[_size.Width * _size.Height];
            _scouted[_position.Y * _size.Width + _position.X] = true;

            _explored = new int[_size.Width * _size.Height];
            _explored[_position.Y * _size.Width + _position.X] = (int)ExploreInfo.Ground;

            bool exploring = true;

            Console.Clear();
            Console.CursorVisible = false;

            while (exploring)
            {
                // Trail (leads back to starting point)

                // CheckSquare();
                // MarkUnexploredDirections();

                // 10:
                // IF UnexploredDirections > 0
                //   WalkUnexploredDirection();
                // ELSE
                //   Backtrack();
                //   GOTO 10
                // 

                CheckSquare();

                Walk();

                if (_state == 11)
                {
                    DrawMap(_grid, _position);
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return "";
                    }

                    _state = 0;
                }
            }

            return "";
        }

        private static void Walk()
        {
            _state = 8;

            var status = _opcodeRunner.Run();

            _pathTrees.Peek().History.Push(_lastDirection);

            if ((Status)status == Status.HitAWall)
            {
                throw new Exception("This shouldn't happen..");
            }

            _position = GetNewPosition();

            _state = 9;
        }

        private static Point GetNewPosition()
        {
            return new Point(_position.X + (_lastDirection == Direction.West ? -1 : _lastDirection == Direction.East ? 1 : 0), _position.Y + (_lastDirection == Direction.North ? -1 : _lastDirection == Direction.South ? 1 : 0));
        }

        private static Direction DetermineWalkingDirection()
        {
            var northPoint = _position.Add(new Point(0, -1));
            var southPoint = _position.Add(new Point(0, 1));
            var westPoint = _position.Add(new Point(-1, 0));
            var eastPoint = _position.Add(new Point(1, 0));

            var positionIndex = _position.Y * _size.Width + _position.X;

            if ((_explored[positionIndex] & (int)ExploreInfo.CanWalkNorth) > 0 && _scouted[northPoint.Y * _size.Width + northPoint.X] == false)
            {
                return Direction.North;
            }
            else if ((_explored[positionIndex] & (int)ExploreInfo.CanWalkSouth) > 0 && _scouted[southPoint.Y * _size.Width + southPoint.X] == false)
            {
                return Direction.South;
            }
            else if ((_explored[positionIndex] & (int)ExploreInfo.CanWalkWest) > 0 && _scouted[westPoint.Y * _size.Width + westPoint.X] == false)
            {
                return Direction.West;
            }
            else if ((_explored[positionIndex] & (int)ExploreInfo.CanWalkEast) > 0 && _scouted[eastPoint.Y * _size.Width + eastPoint.X] == false)
            {
                return Direction.East;
            }

            return Direction.None;
        }

        private static void CheckSquare()
        {
            _state = 0;

            while (_state < 8)
            {
                var status = (Status)_opcodeRunner.Run();

                var index = _position.Y * _size.Width + _position.X;

                if (new[] { 0, 2, 4, 6 }.Contains(_state))
                {
                    var newPosition = GetNewPosition();
                    var newIndex = newPosition.Y * _size.Width + newPosition.X;

                    if (_explored[index] == 0)
                    {
                        _explored[index] = (int)ExploreInfo.Ground;
                    }

                    switch (status)
                    {
                        case Status.HitAWall:
                        {
                            _grid.SetPoint(newPosition, GridData.Wall);
                            _explored[newIndex] = (int)ExploreInfo.Wall;
                            _scouted[newIndex] = true;
                            _state += 2;
                        }
                        break;
                        case Status.MovedOneStep:
                        {
                            _position = newPosition;
                            int canWalk = 0;
                            switch (_lastDirection)
                            {
                                case Direction.North: canWalk = (int)ExploreInfo.CanWalkNorth; break;
                                case Direction.South: canWalk = (int)ExploreInfo.CanWalkSouth; break;
                                case Direction.West: canWalk = (int)ExploreInfo.CanWalkWest; break;
                                case Direction.East: canWalk = (int)ExploreInfo.CanWalkEast; break;
                            }
                            _explored[index] |= canWalk;
                            _scouted[index] = true;
                            _explored[newIndex] = (int)ExploreInfo.Ground;
                            _state += 1;
                        }
                        break;
                        case Status.MovedOneStepFoundOxygenSystem:
                        {
                            _explored[newPosition.Y * _size.Width + newPosition.X] = 1;
                            _scouted[index] = true;
                            _state += 1;
                        }
                        break;
                    }
                }
                else
                {
                    _position = GetNewPosition();
                    _state += 1;
                }
            }

            var explorationData = _explored[_position.Y * _size.Width + _position.X];
            var intersectionInfo = new List<Direction>();

            if ((explorationData & (int)ExploreInfo.CanWalkNorth) > 0) intersectionInfo.Add(Direction.North);
            if ((explorationData & (int)ExploreInfo.CanWalkSouth) > 0) intersectionInfo.Add(Direction.South);
            if ((explorationData & (int)ExploreInfo.CanWalkWest) > 0) intersectionInfo.Add(Direction.West);
            if ((explorationData & (int)ExploreInfo.CanWalkEast) > 0) intersectionInfo.Add(Direction.East);

            if (intersectionInfo.Count > 0)
            {
                _pathTrees.Push(new PathTree());
            }
        }

        private static void Backtrack()
        {
            _state = 10;

            while (_pathTrees.Peek().History.Count > 0)
            {
                _opcodeRunner.Run();
                _position = GetNewPosition();
            }

            _pathTrees.Pop();

            _state = 8;
        }

        private static void DrawMap(Grid grid, Point robot)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

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
                    else if (data == (byte)GridData.Nothing && _explored[y * grid.Size.Width + x] == (int)ExploreInfo.Ground)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.SetCursorPosition(robot.X, robot.Y);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("☺");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static long GetMovementCommand()
        {
            byte movementOrder = 1;
            switch (_state)
            {
                case 0: { movementOrder = 1; } break;
                case 1: { movementOrder = 2; } break;
                case 2: { movementOrder = 2; } break;
                case 3: { movementOrder = 1; } break;
                case 4: { movementOrder = 3; } break;
                case 5: { movementOrder = 4; } break;
                case 6: { movementOrder = 4; } break;
                case 7: { movementOrder = 3; } break;
                case 8:
                {
                    _lastDirection = DetermineWalkingDirection();

                    if (_lastDirection == Direction.None)
                    {
                        _state = 10;
                        return 0;
                    }
                } return (long)_lastDirection;
                case 10:
                {
                    var direction = _pathTrees.Peek().History.Pop();

                    switch (direction)
                    {
                        case Direction.North: return (int)Direction.South;
                        case Direction.South: return (int)Direction.North;
                        case Direction.West: return (int)Direction.East;
                        case Direction.East: return (int)Direction.West;
                    }
                } break;
            }

            _lastDirection = (Direction)movementOrder;
            return movementOrder;
        }
    }
}
