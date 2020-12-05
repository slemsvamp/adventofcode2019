using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day16
{
    public class InputParser
    {
        public static int[] Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var digits = new List<int>();

            foreach (var character in lines[0])
            {
                digits.Add(int.Parse(character.ToString()));
            }

            return digits.ToArray();
        }

        public static string ParseAsString(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}
