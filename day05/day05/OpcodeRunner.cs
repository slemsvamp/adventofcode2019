using System;
using System.Collections.Generic;

namespace day05
{
    public class OpcodeRunner
    {
        public enum ParameterMode : int
        {
            PositionMode = 0,
            ImmediateMode = 1
        }

        public static int Run(List<int> opcodes, int input)
        {
            int output = 0;
            int playhead = 0;

            while (playhead < opcodes.Count)
            {
                int opcode = opcodes[playhead];
                string opcodeString = opcode.ToString();

                int modeA = 0;
                if (opcodeString.Length == 5)
                {
                    modeA = int.Parse(opcodeString.Substring(opcodeString.Length - 5)) / 10000;
                }

                int modeB = 0;
                if (opcodeString.Length >= 4)
                {
                    modeB = int.Parse(opcodeString.Substring(opcodeString.Length - 4)) / 1000;
                }

                int modeC = 0;
                if (opcodeString.Length >= 3)
                {
                    modeC = int.Parse(opcodeString.Substring(opcodeString.Length - 3)) / 100;
                }

                if (opcodeString.Length >= 2)
                {
                    opcode = int.Parse(opcodeString.Substring(opcodeString.Length - 2));
                }

                switch (opcode)
                {
                    case 1:
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        var addressC = opcodes[playhead + 3];

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (modeA == (int)ParameterMode.PositionMode)
                        {
                            opcodes[addressC] = valueA + valueB;
                        }

                        playhead += 4;
                    }
                    break;
                    case 2:
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        var addressC = opcodes[playhead + 3];

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (modeA == (int)ParameterMode.PositionMode)
                        {
                            opcodes[addressC] = valueA * valueB;
                        }

                        playhead += 4;
                    }
                    break;
                    case 3:
                    {
                        var parameterA = opcodes[playhead + 1];

                        opcodes[parameterA] = input;

                        playhead += 2;
                    }
                    break;
                    case 4:
                    {
                        var parameterA = opcodes[playhead + 1];
                        int valueA = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[parameterA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = parameterA;
                        }

                        output = valueA;

                        playhead += 2;
                    }
                    break;
                    case 5: // jump-if-true
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (valueA != 0)
                        {
                            playhead = valueB;
                        }
                        else
                        {
                            playhead += 3;
                        }
                    }
                    break;
                    case 6: // jump-if-false
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (valueA == 0)
                        {
                            playhead = valueB;
                        }
                        else
                        {
                            playhead += 3;
                        }
                    }
                    break;
                    case 7: // less-than
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        var addressC = opcodes[playhead + 3];
                        int valueC = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        //if (modeA == (int)ParameterMode.PositionMode)
                        //{
                        //    valueC = opcodes[addressC];
                        //}
                        //else if (modeA == (int)ParameterMode.ImmediateMode)
                        //{
                        //    valueC = addressC;
                        //}

                        opcodes[addressC] = valueA < valueB ? 1 : 0;

                        playhead += 4;
                    }
                    break;
                    case 8: // equals
                    {
                        var addressA = opcodes[playhead + 1];
                        int valueA = 0;

                        var addressB = opcodes[playhead + 2];
                        int valueB = 0;

                        var addressC = opcodes[playhead + 3];
                        int valueC = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        //if (modeA == (int)ParameterMode.PositionMode)
                        //{
                        //    valueC = opcodes[addressC];
                        //}
                        //else if (modeA == (int)ParameterMode.ImmediateMode)
                        //{
                        //    valueC = addressC;
                        //}

                        opcodes[addressC] = valueA == valueB ? 1 : 0;

                        playhead += 4;
                    }
                    break;
                    case 99:
                    {
                        Console.WriteLine($"Halted, Output={output}");
                        return output;
                    }
                    default:
                    throw new Exception($"Opcode error: {opcode}");
                }

                if (output != 0)
                {
                    Console.WriteLine($"Failed Test, Playhead={playhead}, Output={output}, Opcode={opcodeString}");
                    return output;
                }
            }

            throw new System.Exception("Program finished unexpectedly.");
        }
    }
}
