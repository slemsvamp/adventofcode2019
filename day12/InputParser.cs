using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day12
{
    public class InputParser
    {
        public static List<Vector3> Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var regex = new Regex(@"<x=(?<x>-?\d+), y=(?<y>-?\d+), z=(?<z>-?\d+)>");

            var coordinates = new List<Vector3>();

            foreach (var line in lines)
            {
                // <x=0, y=4, z=0>
                var m = regex.Match(line);
                var coordinate = new Vector3
                {
                    X = long.Parse(m.Groups["x"].Value),
                    Y = long.Parse(m.Groups["y"].Value),
                    Z = long.Parse(m.Groups["z"].Value)
                };
                coordinates.Add(coordinate);
            }

            return coordinates;
        }
    }
}
