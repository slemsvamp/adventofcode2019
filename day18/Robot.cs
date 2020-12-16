using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day18
{
    public struct Robot
    {
        public int Quadrant;
        public Point Position;
        public int Distance;

        public Robot(int quadrant, Point position)
        {
            Quadrant = quadrant;
            Position = position;
            Distance = 0;
        }
    }
}
