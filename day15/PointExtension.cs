using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day15
{
    public static class PointExtension
    {
        public static Point Add(this Point a, Point b)
        {
            return new Point { X = a.X + b.X, Y = a.Y + b.Y };
        }
    }
}
