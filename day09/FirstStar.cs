using System;
using System.Collections.Generic;
using System.Text;

namespace day09
{
    public class FirstStar
    {
        public static string Run(List<long> opcodes)
        {
            // 203 is not right
            // 2662308327 is not right

            OpcodeRunner opcodeRunner = new OpcodeRunner(5000, opcodes);

            opcodeRunner.AddInput(1);

            List<long> outputs = new List<long>();

            while (!opcodeRunner.Halted)
            {
                var output = opcodeRunner.Run();

                if (!opcodeRunner.Halted && output != 0)
                {
                    outputs.Add(output);
                }
            }

            return string.Join(",", outputs);
        }
    }
}
