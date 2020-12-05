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

        private enum State
        {
            Unknown = -1,
            ExploreNorth = 0,
            ReturnExploreNorth = 1,
            ExploreSouth = 2,
            ReturnExploreSouth = 3,
            ExploreWest = 4,
            ReturnExploreWest = 5,
            ExploreEast = 6,
            ReturnExploreEast = 7,
            Walk = 8,
            SuccessfulWalk = 9,
            Backtrack = 10,
        }

        private static int _state;
        private static long _nextInput;

        private static OpcodeRunner _opcodeRunner { get; set; }
        private static Size _size { get; set; }
        private static Grid _grid { get; set; }
        private static Point _position { get; set; }
        public static Point _oxygenSystem { get; set; }
        private static PathTree _backtrack { get; set; }

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
            // 383 too high
            // 389 too high
            // 390 too high

            var opcodes = InputParser.Parse("input.txt");

            _pathTrees = new Stack<PathTree>();
            _pathTrees.Push(new PathTree());

            _opcodeRunner = new OpcodeRunner(opcodes.Count, opcodes, AskForInput);
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

            _state = (int)State.ExploreNorth;
            _nextInput = 1;
            _lastDirection = Direction.North;

            while (exploring)
            {
                var status = _opcodeRunner.Run();

                Record((Status)status);

                _state = (int)NextState((Status)status);

                _nextInput = NextInput();

                if (_nextInput == -999)
                {
                    exploring = false;
                    break;
                }

                _lastDirection = (Direction)_nextInput;
            }

            Console.WriteLine("Farthest In: " + _farthestIn.ToString());

            // ok now let's see, do the oxygen filling thing
            var turnsToFillWithOxygen = FillWithOxygen(_grid, new Point(0, 10));

            return turnsToFillWithOxygen.ToString();
        }

        private static void Record(Status status)
        {
            var index = _position.Y * _size.Width + _position.X;

            if (new[] { State.ExploreNorth, State.ExploreSouth, State.ExploreWest, State.ExploreEast }.Contains((State)_state))
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
                    }
                    break;
                    case Status.MovedOneStepFoundOxygenSystem:
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
                    }
                    break;
                }
            }
            else
            {
                _position = GetNewPosition();
            }
        }

        private static State NextState(Status status)
        {
            if (_state == (int)State.ExploreNorth)
            {
                return status == Status.HitAWall ? State.ExploreSouth : State.ReturnExploreNorth;
            }
            else if (_state == (int)State.ReturnExploreNorth)
            {
                return State.ExploreSouth;
            }
            else if (_state == (int)State.ExploreSouth)
            {
                return status == Status.HitAWall ? State.ExploreWest : State.ReturnExploreSouth;
            }
            else if (_state == (int)State.ReturnExploreSouth)
            {
                return State.ExploreWest;
            }
            else if (_state == (int)State.ExploreWest)
            {
                return status == Status.HitAWall ? State.ExploreEast : State.ReturnExploreWest;
            }
            else if (_state == (int)State.ReturnExploreWest)
            {
                return State.ExploreEast;
            }
            else if (_state == (int)State.ExploreEast)
            {
                return status == Status.HitAWall ? State.Walk : State.ReturnExploreEast;
            }
            else if (_state == (int)State.ReturnExploreEast)
            {
                return State.Walk;
            }
            else if (_state == (int)State.Walk)
            {
                if (_backtrack == null)
                {
                    return State.ExploreNorth;
                }

                return State.Walk;
            }

            return State.Unknown;
        }

        private static long _farthestIn { get; set; }

        private static long NextInput()
        {
            switch ((State)_state)
            {
                case State.ExploreNorth: return 1;
                case State.ReturnExploreNorth: return 2;
                case State.ExploreSouth: return 2;
                case State.ReturnExploreSouth: return 1;
                case State.ExploreWest: return 3;
                case State.ReturnExploreWest: return 4;
                case State.ExploreEast: return 4;
                case State.ReturnExploreEast: return 3;
            }

            if (_state == (int)State.Walk)
            {
                bool wasBacktracking = _backtrack != null;

                if (_backtrack != null && _backtrack.History.Count == 0)
                {
                    _backtrack = null;
                }

                if (_backtrack == null)
                {
                    if (!wasBacktracking && IsIntersection())
                    {
                        _pathTrees.Push(new PathTree());
                    }

                    var unscoutedDirection = UnscoutedDirection();

                    if (unscoutedDirection != Direction.None)
                    {
                        _pathTrees.Peek().History.Push(unscoutedDirection);

                        int length = 0;
                        foreach (var tree in _pathTrees)
                        {
                            length += tree.History.Count;
                        }
                        if (length > _farthestIn)
                        {
                            _farthestIn = length;
                        }

                        return (long)unscoutedDirection;
                    }

                    if (_pathTrees.Count == 0)
                    {
                        return -999;
                    }

                    _backtrack = _pathTrees.Pop();
                }

                return (long)Reverse(_backtrack.History.Pop());
            }

            return -1;
        }

        private static bool IsIntersection()
        {
            var positionIndex = _position.Y * _size.Width + _position.X;

            int paths = 0;

            paths += (_explored[positionIndex] & (int)ExploreInfo.CanWalkNorth) > 0 ? 1 : 0;
            paths += (_explored[positionIndex] & (int)ExploreInfo.CanWalkSouth) > 0 ? 1 : 0;
            paths += (_explored[positionIndex] & (int)ExploreInfo.CanWalkWest) > 0 ? 1 : 0;
            paths += (_explored[positionIndex] & (int)ExploreInfo.CanWalkEast) > 0 ? 1 : 0;

            return paths >= 3;
        }

        public static long AskForInput()
        {
            return _nextInput;
        }

        private static Direction UnscoutedDirection()
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

        private static Direction Reverse(Direction direction)
        {
            switch (direction)
            {
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                case Direction.South: return Direction.North;
                case Direction.North: return Direction.South;
            }
            return Direction.None;
        }

        private static Point GetNewPosition()
        {
            return new Point(_position.X + (_lastDirection == Direction.West ? -1 : _lastDirection == Direction.East ? 1 : 0), _position.Y + (_lastDirection == Direction.North ? -1 : _lastDirection == Direction.South ? 1 : 0));
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

        private static int FillWithOxygen(Grid grid, Point startingPoint)
        {
            HashSet<int> closed = new HashSet<int>();
            List<int> open = new List<int>();
            List<int> nextOpen = new List<int>();

            open.Add(startingPoint.Y * _size.Width + startingPoint.X);

            int minutes = 0;

            while (open.Count > 0)
            {
                nextOpen.Clear();

                foreach (var current in open)
                {
                    closed.Add(current);

                    var point = new Point(current % _size.Width, current / _size.Width);

                    var neighbours = grid.GetNeighbours(point, false);
                
                    foreach (var neighbour in neighbours)
                    {
                        var neighbourIndex = neighbour.Position.Y * _size.Width + neighbour.Position.X;

                        var gridData = grid.Map[neighbourIndex];

                        if (gridData == (byte)GridData.Wall)
                        {
                            closed.Add(neighbourIndex);
                            continue;
                        }

                        if (!closed.Contains(neighbourIndex))
                        {
                            nextOpen.Add(neighbourIndex);
                        }
                    }
                }

                open.Clear();

                foreach (var next in nextOpen)
                {
                    open.Add(next);
                }

                minutes++;
            }

            DrawMap(grid, startingPoint);
            Console.ReadKey(true);

            return minutes - 1;
        }
    }
}
