using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day13
{
    public class InputParser
    {
        public static List<long> Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var opcodes = new List<long>();

            foreach (var line in lines)
            {
                foreach (var opcode in line.Split(new[] { "," }, StringSplitOptions.None))
                {
                    opcodes.Add(long.Parse(opcode));
                }
            }

            return opcodes;
        }
    }
}
