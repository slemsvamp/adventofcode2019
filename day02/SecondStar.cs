using System;
using System.Collections.Generic;
using System.Text;

namespace day02
{
    public class SecondStar
    {
        public static string Run(List<int> opcodes, int expectedOutput = 19690720)
        {
            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    int output = OpcodeRunner.Run(new List<int>(opcodes), noun, verb);

                    if (output == expectedOutput)
                    {
                        return (100 * noun + verb).ToString();
                    }
                }
            }

            throw new Exception("Run ended unexpectedly.");
        }
    }
}
