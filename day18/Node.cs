using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day18
{
    public struct Node
    {
        public char Name;
        public Point Point;

        public Node(char name, Point point)
        {
            Name = name;
            Point = point;
        }
    }
}
