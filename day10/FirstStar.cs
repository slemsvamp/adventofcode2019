using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace day10
{
    public class FirstStar
    {
        private static int RaysToCast = 360;

        public static WinningCoordinate Run(Map map)
        {
            var asteroidCounts = new Dictionary<int, int>();
            Point bestPosition = new Point
            {
                X = -1,
                Y = -1
            };
            var bestCount = 0;

            foreach (var coordinate in map.Coordinates)
            {
                var angles = new HashSet<double>();
                int count = 0;

                foreach (var otherCoordinate in map.Coordinates)
                {
                    if (coordinate == otherCoordinate)
                    {
                        continue;
                    }

                    var angle = Math.Atan2(coordinate.Y - otherCoordinate.Y, coordinate.X - otherCoordinate.X) * 180 / Math.PI;
                    if (!angles.Contains(angle))
                    {
                        angles.Add(angle);
                        count++;
                    }
                }

                if (bestCount < count)
                {
                    bestCount = count;
                    bestPosition = coordinate;
                }

                asteroidCounts[coordinate.Y * map.Size.Width + coordinate.X] = count;
            }

            return new WinningCoordinate
            {
                Coordinate = bestPosition,
                Count = bestCount
            };
        }
    }
}
