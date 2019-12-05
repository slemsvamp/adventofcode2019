using System;
using System.Collections.Generic;
using System.Text;

namespace day05
{
    public class Part02
    {
        public string Run(List<int> opcodes)
        {
            // 1998926 is too high
            // 5346030 is too high

            return OpcodeRunner.Run(opcodes, 5).ToString();
        }
    }
}
