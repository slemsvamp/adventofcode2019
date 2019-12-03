﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day03
{
    public class Part02
    {
        public string Run(List<Wire> wires)
        {
            List<Line> lines = new List<Line>();

            Point centralPoint = new Point(1_000_000, 1_000_000);
            Point topLeft = new Point(1_000_000, 1_000_000);
            Point bottomRight = new Point(1_000_000, 1_000_000);

            foreach (var wire in wires)
            {
                Point wirePoint = centralPoint;
                int lineIndex = 1;

                foreach (var order in wire.Orders)
                {
                    Point start = wirePoint;
                    Point end = wirePoint;
                    var direction = Directionality.Horizontal;

                    switch (order.Letter)
                    {
                        case "U":
                        {
                            wirePoint.Y -= order.Number;
                            direction = Directionality.Vertical;
                            start = wirePoint;
                        }
                        break;
                        case "D":
                        {
                            wirePoint.Y += order.Number;
                            direction = Directionality.Vertical;
                            end = wirePoint;
                        }
                        break;
                        case "L":
                        {
                            wirePoint.X -= order.Number;
                            start = wirePoint;
                        }
                        break;
                        case "R":
                        {
                            wirePoint.X += order.Number;
                            end = wirePoint;
                        }
                        break;
                    }

                    if (wirePoint.Y < topLeft.Y)
                    {
                        topLeft.Y = wirePoint.Y;
                    }
                    if (wirePoint.X < topLeft.X)
                    {
                        topLeft.X = wirePoint.X;
                    }
                    if (wirePoint.Y > bottomRight.Y)
                    {
                        bottomRight.Y = wirePoint.Y;
                    }
                    if (wirePoint.X > bottomRight.X)
                    {
                        bottomRight.X = wirePoint.X;
                    }

                    lines.Add(new Line
                    {
                        LineIndex = lineIndex++,
                        Start = start,
                        End = end,
                        Direction = direction,
                        WireIndex = wire.Index
                    });
                }
            }

            Size canvasSize = new Size
            {
                Height = bottomRight.Y - topLeft.Y + 1,
                Width = bottomRight.X - topLeft.X + 1
            };

            var crossings = new List<Point>();

            int smallestLineIndex = int.MaxValue;

            for (int lineIndex = 0; lineIndex < lines.Count - 1; lineIndex++)
            {
                var line = lines[lineIndex];

                for (var otherLineIndex = lineIndex + 1; otherLineIndex < lines.Count; otherLineIndex++)
                {
                    var otherLine = lines[otherLineIndex];

                    if (line.WireIndex == otherLine.WireIndex)
                    {
                        continue;
                    }

                    if (line.Direction == otherLine.Direction)
                    {
                        continue;
                    }

                    if (line.Direction == Directionality.Horizontal)
                    {
                        if (line.Start.Y >= otherLine.Start.Y && line.Start.Y <= otherLine.End.Y &&
                            otherLine.Start.X >= line.Start.X && otherLine.Start.X <= line.End.X)
                        {
                            crossings.Add(new Point
                            {
                                X = otherLine.End.X,
                                Y = line.Start.Y
                            });

                            if (line.LineIndex < smallestLineIndex)
                            {
                                smallestLineIndex = line.LineIndex;
                            }
                            if (otherLine.LineIndex < smallestLineIndex)
                            {
                                smallestLineIndex = otherLine.LineIndex;
                            }
                        }
                    }
                    else
                    {
                        if (otherLine.Start.Y >= line.Start.Y && otherLine.Start.Y <= line.End.Y &&
                            line.Start.X >= otherLine.Start.X && line.Start.X <= otherLine.End.X)
                        {
                            crossings.Add(new Point
                            {
                                X = line.End.X,
                                Y = otherLine.Start.Y
                            });

                            if (line.LineIndex < smallestLineIndex)
                            {
                                smallestLineIndex = line.LineIndex;
                            }
                            if (otherLine.LineIndex < smallestLineIndex)
                            {
                                smallestLineIndex = otherLine.LineIndex;
                            }
                        }
                    }
                }
            }

            Point closestToCenter = Point.Empty;
            int distance = int.MaxValue;

            Point center = new Point
            {
                X = topLeft.X + (canvasSize.Width / 2),
                Y = topLeft.Y + (canvasSize.Height / 2)
            };

            foreach (var crossing in crossings)
            {
                var distanceToStart = Math.Abs(crossing.X - 1_000_000) + Math.Abs(crossing.Y - 1_000_000);
                if (distanceToStart == 0)
                {
                    continue;
                }

                var manhattanDistance = Math.Abs(crossing.X - 1_000_000) + Math.Abs(crossing.Y - 1_000_000);
                if (manhattanDistance < distance)
                {
                    distance = manhattanDistance;
                    closestToCenter = crossing;
                }
            }

            return smallestLineIndex.ToString();
        }
    }
}
