using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace day05
{
    public class InputParser
    {
        public static List<int> Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var numbers = new List<int>();

            foreach (var line in lines)
            {
                foreach (var number in line.Split(new[] { "," }, StringSplitOptions.None))
                {
                    numbers.Add(int.Parse(number));
                }
            }

            return numbers;
        }
    }
}
