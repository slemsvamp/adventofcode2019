using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day18
{
    public struct Robot
    {
        public Point Position;
        public int Distance;

        public Robot(Point position, int distance)
        {
            Position = position;
            Distance = distance;
        }
    }
}
