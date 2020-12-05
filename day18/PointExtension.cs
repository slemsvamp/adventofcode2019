using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day18
{
    public static class PointExtension
    {
        public static Point Add(this Point a, int x, int y)
        {
            return new Point { X = a.X + x, Y = a.Y + y };
        }
    }
}
