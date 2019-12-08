using System;
using System.Collections.Generic;
using System.Text;

namespace day05
{
    public class Part01
    {
        public string Run(List<int> opcodes)
        {
            return OpcodeRunner.Run(opcodes, 1).ToString();
        }
    }
}
