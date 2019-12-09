using System;
using System.Collections.Generic;
using System.Text;

namespace day09
{
    public class SecondStar
    {
        public static string Run(List<long> opcodes)
        {
            OpcodeRunner opcodeRunner = new OpcodeRunner(5000, opcodes);

            opcodeRunner.AddInput(2);

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
