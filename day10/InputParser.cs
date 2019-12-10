using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day10
{
    public class InputParser
    {
        public static Map Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var map = new Map
            {
                Asteroids = new bool[0],
                Size = new Size
                {
                    Width = -1,
                    Height = -1
                }
            };

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];

                if (map.Size.Width < 0)
                {
                    map.Size = new Size
                    {
                        Width = line.Length,
                        Height = lines.Length
                    };

                    map.Coordinates = new List<Point>();

                    map.Asteroids = new bool[map.Size.Width * map.Size.Height];
                }

                for (int columnIndex = 0; columnIndex < line.Length; columnIndex++)
                {
                    var hit = line[columnIndex] == '#';

                    map.Asteroids[lineIndex * map.Size.Width + columnIndex] = hit;

                    if (hit)
                    {
                        map.Coordinates.Add(new Point
                        {
                            X = columnIndex,
                            Y = lineIndex
                        });
                    }
                }
            }

            return map;
        }
    }
}
