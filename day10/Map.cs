using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day10
{
    public class Map
    {
        public bool[] Asteroids { get; set; }
        public List<Point> Coordinates { get; set; }
        public Size Size { get; set; }
    }
}
