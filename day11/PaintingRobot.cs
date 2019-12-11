using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day11
{
    public enum Turn : int
    {
        Left = 0,
        Right = 1
    }

    public enum PaintColor : int
    {
        Black = 0,
        White = 1
    }

    public enum Direction : int
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public class PaintingRobot
    {
        private HashSet<int> _uniqueTilesPainted;
        private readonly int[] _timesPainted;
        private readonly Size _gridSize;
        private readonly List<long> _opcodes;
        private readonly bool _shouldDraw;
        private int _facing { get; set; }
        private Coordinate _position { get; set; }

        public PaintingRobot(List<long> opcodes, Coordinate startingPosition, Size gridSize, bool shouldDraw = false)
        {
            _position = startingPosition;
            _opcodes = opcodes;
            _facing = 0;
            _timesPainted = new int[gridSize.Width * gridSize.Height];
            _uniqueTilesPainted = new HashSet<int>();
            _gridSize = gridSize;
            _shouldDraw = shouldDraw;
        }

        public int Run(int[] grid)
        {
            var opcodeRunner = new OpcodeRunner(5000, _opcodes);

            while (!opcodeRunner.Halted)
            {
                // I want to paint it...
                var positionIndex = _position.Y * _gridSize.Width + _position.X;

                var beforePaint = (PaintColor)grid[positionIndex];
                opcodeRunner.AddInput((int)beforePaint);

                grid[positionIndex] = (int)opcodeRunner.Run();
                _timesPainted[positionIndex] = _timesPainted[positionIndex] + 1;
                if (!_uniqueTilesPainted.Contains(positionIndex))
                {
                    _uniqueTilesPainted.Add(positionIndex);
                }

                // Turn
                var turn = (Turn)opcodeRunner.Run();

                if (turn == Turn.Left)
                {
                    _facing -= 1;
                }
                else
                {
                    _facing += 1;
                }

                if (_facing < 0)
                {
                    _facing = 3;
                }
                else if (_facing > 3)
                {
                    _facing = 0;
                }

                // Move forward
                switch ((Direction)_facing)
                {
                    case Direction.Right:
                    {
                        _position.X = _position.X + 1;
                    }
                    break;
                    case Direction.Left:
                    {
                        _position.X = _position.X - 1;
                    }
                    break;
                    case Direction.Up:
                    {
                        _position.Y = _position.Y - 1;
                    }
                    break;
                    case Direction.Down:
                    {
                        _position.Y = _position.Y + 1;
                    }
                    break;
                }
            }

            if (_shouldDraw)
            {
                var margin = new Coordinate(2, 5);

                for (int y = 0; y < _gridSize.Height; y++)
                {
                    for (int x = 0; x < _gridSize.Width; x++)
                    {
                        Console.SetCursorPosition(margin.X + x, margin.Y + y);

                        if (grid[y * _gridSize.Width + x] == (int)PaintColor.White)
                        {
                            Console.Write("░");
                        }
                    }
                }
            }

            return _uniqueTilesPainted.Count;
        }
    }
}
