using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day04
{
    public class InputParser
    {
        public static object Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            foreach (var line in lines)
            {
            }

            return lines;
        }
    }
}
