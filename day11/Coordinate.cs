using System;
using System.Collections.Generic;
using System.Text;

namespace day11
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordinate Empty
        {
            get
            {
                return new Coordinate(0, 0);
            }
        }
    }
}
