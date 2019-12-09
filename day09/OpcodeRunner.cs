using System;
using System.Collections.Generic;

namespace day09
{
    public class OpcodeRunner
    {
        public bool Halted { get; set; }

        private int _playhead = 0;
        private long _output = 0;
        private long _relativeBase = 0;

        private Queue<long> _input;
        private long[] _opcodes;

        public enum ParameterMode : long
        {
            PositionMode = 0,
            ImmediateMode = 1,
            RelativeMode = 2
        }

        public OpcodeRunner(int memorySize, List<long> opcodes)
        {
            Halted = false;
            _output = 0;
            _playhead = 0;

            _opcodes = new long[memorySize];

            for (int memoryIndex = 0; memoryIndex < opcodes.Count; memoryIndex++)
            {
                _opcodes[memoryIndex] = opcodes[memoryIndex];
            }

            _input = new Queue<long>();
        }

        public void AddInput(long input)
        {
            _input.Enqueue(input);
        }

        public long Run()
        {
            while (_playhead < _opcodes.Length)
            {
                long opcode = _opcodes[_playhead];
                string opcodeString = opcode.ToString();

                long modeA = 0;
                if (opcodeString.Length == 5)
                {
                    modeA = long.Parse(opcodeString.Substring(opcodeString.Length - 5)) / 10000;
                }

                long modeB = 0;
                if (opcodeString.Length >= 4)
                {
                    modeB = long.Parse(opcodeString.Substring(opcodeString.Length - 4)) / 1000;
                }

                long modeC = 0;
                if (opcodeString.Length >= 3)
                {
                    modeC = long.Parse(opcodeString.Substring(opcodeString.Length - 3)) / 100;
                }

                if (opcodeString.Length >= 2)
                {
                    opcode = long.Parse(opcodeString.Substring(opcodeString.Length - 2));
                }

                switch (opcode)
                {
                    case 1:
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        var addressC = _opcodes[_playhead + 3];

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (modeA == (long)ParameterMode.PositionMode)
                        {
                            _opcodes[(int)addressC] = valueA + valueB;
                        }
                        else if (modeA == (long)ParameterMode.RelativeMode)
                        {
                            _opcodes[(int)(addressC + _relativeBase)] = valueA + valueB;
                        }

                        _playhead += 4;
                    }
                    break;
                    case 2:
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        var addressC = _opcodes[_playhead + 3];

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (modeA == (long)ParameterMode.PositionMode)
                        {
                            _opcodes[(int)addressC] = valueA * valueB;
                        }
                        else if (modeA == (long)ParameterMode.RelativeMode)
                        {
                            _opcodes[(int)(addressC + _relativeBase)] = valueA * valueB;
                        }

                        _playhead += 4;
                    }
                    break;
                    case 3:
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = addressA + _relativeBase;
                        }

                        _opcodes[(int)valueA] = _input.Dequeue();

                        _playhead += 2;
                    }
                    break;
                    case 4:
                    {
                        var parameterA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)parameterA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = parameterA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(parameterA + _relativeBase)];
                        }

                        _output = valueA;
                        _playhead += 2;

                        return _output;
                    }
                    case 5: // jump-if-true
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (valueA != 0)
                        {
                            _playhead = (int)valueB;
                        }
                        else
                        {
                            _playhead += 3;
                        }
                    }
                    break;
                    case 6: // jump-if-false
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (valueA == 0)
                        {
                            _playhead = (int)valueB;
                        }
                        else
                        {
                            _playhead += 3;
                        }
                    }
                    break;
                    case 7: // less-than
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        var addressC = _opcodes[_playhead + 3];
                        long valueC = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (modeA == (long)ParameterMode.PositionMode)
                        {
                            valueC = addressC;
                        }
                        else if (modeA == (long)ParameterMode.ImmediateMode)
                        {
                            valueC = addressC;
                        }
                        else if (modeA == (long)ParameterMode.RelativeMode)
                        {
                            valueC = addressC + _relativeBase;
                        }

                        _opcodes[(int)valueC] = valueA < valueB ? 1 : 0;

                        _playhead += 4;
                    }
                    break;
                    case 8: // equals
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        long valueB = 0;

                        var addressC = _opcodes[_playhead + 3];
                        long valueC = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        if (modeB == (long)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[(int)addressB];
                        }
                        else if (modeB == (long)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }
                        else if (modeB == (long)ParameterMode.RelativeMode)
                        {
                            valueB = _opcodes[(int)(addressB + _relativeBase)];
                        }

                        if (modeA == (long)ParameterMode.PositionMode)
                        {
                            valueC = addressC;
                        }
                        else if (modeA == (long)ParameterMode.ImmediateMode)
                        {
                            valueC = addressC;
                        }
                        else if (modeA == (long)ParameterMode.RelativeMode)
                        {
                            valueC = addressC + _relativeBase;
                        }

                        _opcodes[(int)valueC] = valueA == valueB ? 1 : 0;

                        _playhead += 4;
                    }
                    break;
                    case 9:
                    {
                        var addressA = _opcodes[_playhead + 1];
                        long valueA = 0;

                        if (modeC == (long)ParameterMode.PositionMode)
                        {
                             valueA = _opcodes[(int)addressA];
                        }
                        else if (modeC == (long)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }
                        else if (modeC == (long)ParameterMode.RelativeMode)
                        {
                            valueA = _opcodes[(int)(addressA + _relativeBase)];
                        }

                        _relativeBase += valueA;

                        _playhead += 2;
                    }
                    break;
                    case 99:
                    {
                        Halted = true;
                        return _output;
                    }
                    default:
                    throw new Exception($"Opcode error: {opcode}");
                }
            }

            throw new System.Exception("Program finished unexpectedly.");
        }

        //internal void SetOpcodes(List<long> opcodes)
        //{
        //    _opcodes = opcodes;
        //}
    }
}
