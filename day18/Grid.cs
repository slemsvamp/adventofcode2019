using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace day18
{
    public class Grid : IGrid
    {
        private readonly Size _size;

        public Size Size => _size;
        
        public char[] Map { get; set; }

        public Grid(string[] map, Size size)
        {
            _size = size;
            
            Map = new char[size.Width * size.Height];

            for (int y = 0; y < size.Height; y++)
                for (int x = 0; x < size.Width; x++)
                    Map[x + y * size.Width] = map[y][x];
        }

        public List<RetraceData> GenerateRetraces(Dictionary<char, Point> keyPositions, Point characterPosition)
        {
            var pathfinder = new Pathfinder();
            var retraces = new List<RetraceData>();

            foreach (var endKeyValuePair in keyPositions)
            {
                var endKey = endKeyValuePair.Key;
                var endPosition = endKeyValuePair.Value;

                var retracePath = pathfinder.FindPath(characterPosition, endPosition, this);

                retraces.Add(new RetraceData
                {
                    From = '@',
                    To = endKey,
                    Doors = retracePath.Doors,
                    Length = retracePath.Path.Length
                });
            }

            foreach (var startKeyValuePair in keyPositions)
            {
                var startKey = startKeyValuePair.Key;
                var startPosition = startKeyValuePair.Value;

                foreach (var endKeyValuePair in keyPositions)
                {
                    var endKey = endKeyValuePair.Key;
                    var endPosition = endKeyValuePair.Value;

                    var pair = retraces.Where(r => r.From == endKey && r.To == startKey).SingleOrDefault();

                    if (startKey == endKey)
                    {
                        continue;
                    }
                    else if (pair != null)
                    {
                        retraces.Add(new RetraceData
                        {
                            From = startKey,
                            To = endKey,
                            Doors = pair.Doors,
                            Length = pair.Length
                        });

                        continue;
                    }

                    var retracePath = pathfinder.FindPath(startPosition, endPosition, this);

                    retraces.Add(new RetraceData
                    {
                        From = startKey,
                        To = endKey,
                        Doors = retracePath.Doors,
                        Length = retracePath.Path.Length
                    });
                }
            }

            return retraces;
        }

        public void SetPoint(Point point, char data)
        {
            Map[point.Y * _size.Width + point.X] = data;
        }

        public int GetMaximumHeapSize()
        {
            return _size.Width * _size.Height;
        }

        public PathfinderNode[] GetNeighbours(Point point, bool allowDiagonals)
        {
            if (point.X > 0 && point.X < _size.Width && point.Y > 0 && point.Y < _size.Height)
            {
                int wallCost = int.MaxValue;

                Point up = point.Add(0, -1), down = point.Add(0, 1), left = point.Add(-1, 0), right = point.Add(1, 0);
                int upCost = wallCost, downCost = wallCost, leftCost = wallCost, rightCost = wallCost;

                var upData = Map[up.Y * _size.Width + up.X];
                var downData = Map[down.Y * _size.Width + down.X];
                var leftData = Map[left.Y * _size.Width + left.X];
                var rightData = Map[right.Y * _size.Width + right.X];

                if (upData == '.' || upData != '#')
                {
                    upCost = 1;
                }

                if (downData == '.' || downData != '#')
                {
                    downCost = 1;
                }

                if (leftData == '.' || leftData != '#')
                {
                    leftCost = 1;
                }

                if (rightData == '.' || rightData != '#')
                {
                    rightCost = 1;
                }

                var isDoor = new Func<char, bool>(c => c != '.' && c != '#' && c != '@' && c.ToString().ToUpper() == c.ToString());

                return new PathfinderNode[4]
                {
                    new PathfinderNode(up, isDoor(upData) ? upData : '\0', upCost),
                    new PathfinderNode(down, isDoor(downData) ? downData : '\0', downCost),
                    new PathfinderNode(left, isDoor(leftData) ? leftData : '\0', leftCost),
                    new PathfinderNode(right, isDoor(rightData) ? rightData : '\0', rightCost)
                };
            }

            return new PathfinderNode[4]
            {
                new PathfinderNode(point.Add(0, -1), '\0', int.MaxValue),
                new PathfinderNode(point.Add(0, 1), '\0', int.MaxValue),
                new PathfinderNode(point.Add(-1, 0), '\0', int.MaxValue),
                new PathfinderNode(point.Add(1, 0), '\0', int.MaxValue)
            };
        }

        public PathfinderNode GetNodeFromWorldPoint(Point point)
        {
            var pointData = Map[point.Y * _size.Width + point.Y];
            var isDoor = pointData != '.' && pointData != '#' && pointData.ToString().ToUpper() == pointData.ToString();

            return new PathfinderNode(point, isDoor ? pointData : '\0', pointData);
        }
    }
}
