using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace day10
{
    public class SecondStar
    {
        private class AsteroidData
        {
            public int Index { get; set; }
            public double Angle { get; set; }
            public int Distance { get; set; }
            public Point Coordinate { get; set; }
        }

        private static double Angle(Point from, Point to)
        {
            var arcTangent = Math.Atan2(to.Y - from.Y, to.X - from.X) + Math.PI;
            return ((arcTangent * 180.0 / Math.PI) + 270) % 360.0;
        }

        public static int Run(Point startingPoint, Map map)
        {
            var asteroids = new Dictionary<int, AsteroidData>();

            foreach (var asteroid in map.Coordinates)
            {
                if (asteroid.X == startingPoint.X && asteroid.Y == startingPoint.Y)
                {
                    continue;
                }

                var angle = Angle(startingPoint, asteroid);
                var distance = Math.Abs(startingPoint.X - asteroid.X) + Math.Abs(startingPoint.Y - asteroid.Y);

                var asteroidIndex = asteroid.Y * map.Size.Width + asteroid.X;

                asteroids[asteroidIndex] = new AsteroidData
                {
                    Index = asteroidIndex,
                    Angle = angle,
                    Distance = distance,
                    Coordinate = asteroid
                };
            }

            var uniqueAngles = asteroids.Values.Select(a => a.Angle).Distinct().OrderBy(a => a).ToList();

            double nudge = 0.0000000001;
            double startingAngle = 0.0;

            double currentAngle = startingAngle;
            int hits = 0;

            while (true)
            {

                foreach (var uniqueAngle in uniqueAngles)
                {
                    if (currentAngle >= 360.0)
                    {
                        currentAngle = 0;
                    }

                    double diff = uniqueAngle - currentAngle;

                    if (diff >= 0)
                    {
                        currentAngle = uniqueAngle;

                        var asteroidsHitAtAngle = asteroids.Where(kvp => kvp.Value.Angle == currentAngle);
                        var closestAsteroid = asteroidsHitAtAngle.OrderBy(kvp => kvp.Value.Distance).Select(kvp => kvp.Value).FirstOrDefault();

                        if (closestAsteroid == null)
                        {
                            // we've already taken out all the asteroids here, don't count this one
                        }
                        else
                        {
                            asteroids.Remove(closestAsteroid.Index);
                            hits++;

                            if (hits == 200)
                            {
                                return closestAsteroid.Coordinate.X * 100 + closestAsteroid.Coordinate.Y;
                            }
                        }

                        currentAngle += nudge;
                    }
                }
            }
        }
    }
}
