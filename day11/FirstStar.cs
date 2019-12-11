using System.Collections.Generic;
using System.Drawing;

namespace day11
{
    public class FirstStar
    {
        public static string Run(List<long> opcodes)
        {
            var _gridSize = new Size(10000, 10000);
            int[] grid = new int[_gridSize.Width * _gridSize.Height];
            var robot = new PaintingRobot(opcodes, new Coordinate(5000, 5000), _gridSize);
            var result = robot.Run(grid);
            return result.ToString();
        }
    }
}
