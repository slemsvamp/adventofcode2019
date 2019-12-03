using System;
using System.Collections.Generic;
using System.Drawing;

namespace day03
{
    public class Part02
    {
        public string Run(List<Wire> wires, List<Point> crossings)
        {
            List<Line> lines = new List<Line>();

            Point centralPoint = new Point(1_000_000, 1_000_000);
            Point topLeft = new Point(1_000_000, 1_000_000);
            Point bottomRight = new Point(1_000_000, 1_000_000);

            int smallestWireLength = int.MaxValue;

            foreach (var crossing in crossings)
            {
                if (crossing.X == centralPoint.X && crossing.Y == centralPoint.Y)
                {
                    continue;
                }

                var wiresTotalLength = CalculateWiresForCrossing(centralPoint, crossing, wires);

                if (smallestWireLength > wiresTotalLength)
                {
                    smallestWireLength = wiresTotalLength;
                }
            }

            return smallestWireLength.ToString();
        }

        private int CalculateWiresForCrossing(Point centralPoint, Point crossing, List<Wire> wires)
        {
            int wireTotal = 0;

            foreach (var wire in wires)
            {
                int wireLength = 0;

                Point wirePoint = centralPoint;

                foreach (var order in wire.Orders)
                {
                    Point start = wirePoint;
                    Point end = wirePoint;

                    switch (order.Letter)
                    {
                        case "U":
                        {
                            wirePoint.Y -= order.Number;
                        }
                        break;
                        case "D":
                        {
                            wirePoint.Y += order.Number;
                        }
                        break;
                        case "L":
                        {
                            wirePoint.X -= order.Number;
                        }
                        break;
                        case "R":
                        {
                            wirePoint.X += order.Number;
                        }
                        break;
                    }

                    end = wirePoint;

                    var line = new Line
                    {
                        Start = start,
                        End = end,
                        WireIndex = wire.Index
                    };

                    if (LineIntersectsWithCrossing(order.Letter, line, crossing))
                    {
                        wireLength += Math.Abs(crossing.X - line.Start.X) + Math.Abs(crossing.Y - line.Start.Y);
                        break;
                    }
                    else
                    {
                        wireLength += order.Number;
                    }
                }

                wireTotal += wireLength;
            }

            return wireTotal;
        }

        private bool LineIntersectsWithCrossing(string direction, Line lineIn, Point crossing)
        {
            var line = new Line
            {
                Start = direction == "U" || direction == "L" ? lineIn.End : lineIn.Start,
                End = direction == "U" || direction == "L" ? lineIn.Start : lineIn.End
            };

            if (crossing.X >= line.Start.X && crossing.X <= line.End.X &&
                crossing.Y >= line.Start.Y && crossing.Y <= line.End.Y)
            {
                return true;
            }
            return false;
        }
    }
}
