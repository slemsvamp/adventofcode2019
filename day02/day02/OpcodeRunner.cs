using System.Collections.Generic;

namespace day02
{
    public class OpcodeRunner
    {
        public static int Run(List<int> opcodes, int noun, int verb)
        {
            opcodes[1] = noun;
            opcodes[2] = verb;

            int playhead = 0;

            while (playhead < opcodes.Count)
            {
                int opcode = opcodes[playhead];

                switch (opcode)
                {
                    case 1:
                    {
                        var addressA = opcodes[playhead + 1];
                        var valueA = opcodes[addressA];

                        var addressB = opcodes[playhead + 2];
                        var valueB = opcodes[addressB];

                        var addressC = opcodes[playhead + 3];

                        opcodes[addressC] = valueA + valueB;

                        playhead += 4;
                    }
                    break;
                    case 2:
                    {
                        var addressA = opcodes[playhead + 1];
                        var valueA = opcodes[addressA];

                        var addressB = opcodes[playhead + 2];
                        var valueB = opcodes[addressB];

                        var addressC = opcodes[playhead + 3];

                        opcodes[addressC] = valueA * valueB;

                        playhead += 4;
                    }
                    break;
                    case 99:
                    {
                        return opcodes[0];
                    }
                    default:
                    break;
                }
            }

            throw new System.Exception("Program finished unexpectedly.");
        }
    }
}
