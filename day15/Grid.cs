using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day15
{
    public enum GridData : byte
    {
        Nothing = 0,
        Wall = 1,
        OxygenSystem = 2
    }

    public class Grid : IGrid
    {
        private Size _size { get; set; }
        public byte[] Map { get; set; }

        public Size Size
        {
            get
            {
                return _size;
            }
        }

        public Grid(Size size)
        {
            _size = size;
            Map = new byte[size.Width * size.Height];
        }

        public void SetPoint(Point point, GridData data)
        {
            Map[point.Y * _size.Width + point.X] = (byte)data;
        }

        public int GetMaximumHeapSize()
        {
            throw new NotImplementedException();
        }

        public PathFinderNode[] GetNeighbours(Point point, bool allowDiagonals)
        {
            throw new NotImplementedException();
        }

        public PathFinderNode GetNodeFromWorldPoint(Point point)
        {
            throw new NotImplementedException();
        }
    }
}
