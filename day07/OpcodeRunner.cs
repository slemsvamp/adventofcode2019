using System;
using System.Collections.Generic;

namespace day07
{
    public class OpcodeRunner
    {
        public bool Halted { get; set; }

        private int _playhead = 0;
        private int _output = 0;

        private Queue<int> _input;
        private List<int> _opcodes;

        public enum ParameterMode : int
        {
            PositionMode = 0,
            ImmediateMode = 1
        }

        public OpcodeRunner(List<int> opcodes)
        {
            Halted = false;
            _output = 0;
            _playhead = 0;
            _opcodes = new List<int>(opcodes);
            _input = new Queue<int>();
        }

        public void AddInput(int input)
        {
            _input.Enqueue(input);
        }

        public int Run()
        {
            while (_playhead < _opcodes.Count)
            {
                int opcode = _opcodes[_playhead];
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
                        var addressA = _opcodes[_playhead + 1];
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        var addressC = _opcodes[_playhead + 3];

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (modeA == (int)ParameterMode.PositionMode)
                        {
                            _opcodes[addressC] = valueA + valueB;
                        }

                        _playhead += 4;
                    }
                    break;
                    case 2:
                    {
                        var addressA = _opcodes[_playhead + 1];
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        var addressC = _opcodes[_playhead + 3];

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (modeA == (int)ParameterMode.PositionMode)
                        {
                            _opcodes[addressC] = valueA * valueB;
                        }

                        _playhead += 4;
                    }
                    break;
                    case 3:
                    {
                        var parameterA = _opcodes[_playhead + 1];

                        _opcodes[parameterA] = _input.Dequeue();

                        _playhead += 2;
                    }
                    break;
                    case 4:
                    {
                        var parameterA = _opcodes[_playhead + 1];
                        int valueA = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[parameterA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = parameterA;
                        }

                        _output = valueA;
                        _playhead += 2;

                        //if (_output != 0)
                        //{
                            return _output;
                        //}
                    }
                    break;
                    case 5: // jump-if-true
                    {
                        var addressA = _opcodes[_playhead + 1];
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (valueA != 0)
                        {
                            _playhead = valueB;
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
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        if (valueA == 0)
                        {
                            _playhead = valueB;
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
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        var addressC = _opcodes[_playhead + 3];

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        _opcodes[addressC] = valueA < valueB ? 1 : 0;

                        _playhead += 4;
                    }
                    break;
                    case 8: // equals
                    {
                        var addressA = _opcodes[_playhead + 1];
                        int valueA = 0;

                        var addressB = _opcodes[_playhead + 2];
                        int valueB = 0;

                        var addressC = _opcodes[_playhead + 3];
                        int valueC = 0;

                        if (modeC == (int)ParameterMode.PositionMode)
                        {
                            valueA = _opcodes[addressA];
                        }
                        else if (modeC == (int)ParameterMode.ImmediateMode)
                        {
                            valueA = addressA;
                        }

                        if (modeB == (int)ParameterMode.PositionMode)
                        {
                            valueB = _opcodes[addressB];
                        }
                        else if (modeB == (int)ParameterMode.ImmediateMode)
                        {
                            valueB = addressB;
                        }

                        _opcodes[addressC] = valueA == valueB ? 1 : 0;

                        _playhead += 4;
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

        internal void SetOpcodes(List<int> opcodes)
        {
            _opcodes = opcodes;
        }
    }
}
