using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day11
{
    public class SecondStar
    {
        public static void Run(List<long> opcodes)
        {
            var _gridSize = new Size(50, 8);
            int[] grid = new int[_gridSize.Width * _gridSize.Height];
            var startingPosition = new Coordinate(0, 0);
            grid[startingPosition.Y * _gridSize.Width + startingPosition.X] = (int)PaintColor.White;

            var robot = new PaintingRobot(opcodes, startingPosition, _gridSize, true);
            robot.Run(grid);
        }
    }
}
