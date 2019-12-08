using System;
using System.Collections.Generic;
using System.Text;

namespace day02
{
    public class FirstStar
    {
        public static string Run(List<int> opcodes)
        {
            return OpcodeRunner.Run(new List<int>(opcodes), 12, 2).ToString();
        }
    }
}
