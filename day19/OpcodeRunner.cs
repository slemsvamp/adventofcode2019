using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace day19
{
    public class OpcodeRunner
    {
        public bool Halted { get; set; }

        private int _playhead = 0;
        private long _output = 0;
        private long _relativeBase = 0;

        private Queue<long> _input;
        private long[] _opcodes;

        private Func<long> AskForInput;

        public enum ParameterMode : long
        {
            PositionMode = 0,
            ImmediateMode = 1,
            RelativeMode = 2
        }

        public OpcodeRunner(int memorySize, List<long> opcodes, Func<long> askForInput)
        {
            Halted = false;

            _output = 0;
            _playhead = 0;

            AskForInput = askForInput;

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

        public long ReadNext()
        {
            return _opcodes[_playhead++];
        }

        public long[] ReadOpcodes()
        {
            return _opcodes;
        }

        public void SetOpcodes(long[] opcodes)
        {
            _opcodes = opcodes;
        }

        public int ReadPlayhead()
        {
            return _playhead;
        }

        public void SetPlayhead(int playhead)
        {
            _playhead = playhead;
        }

        public Queue<long> ReadInput()
        {
            return _input;
        }

        public void SetInput(Queue<long> input)
        {
            _input = input;
        }

        public long Run()
        {
            while (_playhead < _opcodes.Length)
            {
                var intCode = ReadNext();
                var parameterModes = ParameterModes(intCode);
                var instruction = intCode > 9 ? long.Parse(intCode.ToString().Substring(intCode.ToString().Length - 2)) : intCode;

                Debug.Write($"[{_playhead}]: ");

                switch (instruction)
                {
                    case 1:
                    {
                        Debug.Write($"ADD => ");
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Addition(parameters);
                        Debug.Write($"{parameters[0]}+{parameters[1]}={_opcodes[parameters[2]]}");
                    }
                    break;
                    case 2:
                    {
                        Debug.Write($"MUL => ");
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Multiplication(parameters);
                        Debug.Write($"{parameters[0]}*{parameters[1]}={_opcodes[parameters[2]]}");
                    }
                    break;
                    case 3:
                    {
                        Debug.Write($"INP => ");
                        var parameters = ReadNextParameters(parameterModes, "W", 1);
                        Input(parameters);
                        Debug.Write($"{_opcodes[parameters[0]]}(@{parameters[0]})");
                    }
                    break;
                    case 4:
                    {
                        Debug.Write($"OUT => ");
                        var parameters = ReadNextParameters(parameterModes, "R", 1);
                        Output(parameters);
                        Debug.Write($"{_output}");
                    }
                    return _output;
                    case 5:
                    {
                        Debug.Write($"JMP => ");
                        var parameters = ReadNextParameters(parameterModes, "RR", 2);
                        JumpIfTrue(parameters);
                        Debug.Write($"{parameters[0]}=TRUE? JMP={parameters[1]}");
                    }
                    break;
                    case 6:
                    {
                        Debug.Write($"JPF => ");
                        var parameters = ReadNextParameters(parameterModes, "RR", 2);
                        JumpIfFalse(parameters);
                        Debug.Write($"{parameters[0]}=FALSE? JMP={parameters[1]}");
                    }
                    break;
                    case 7:
                    {
                        Debug.Write($"LTN => ");
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        LessThan(parameters);
                        Debug.Write($"{parameters[0]}<{parameters[1]}={_opcodes[parameters[2]]}");
                    }
                    break;
                    case 8:
                    {
                        Debug.Write($"GTN => ");
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Equal(parameters);
                        Debug.Write($"{parameters[0]}>{parameters[1]}={_opcodes[parameters[2]]}");
                    }
                    break;
                    case 9:
                    {
                        Debug.Write($"SRB => ");
                        var parameters = ReadNextParameters(parameterModes, "R", 1);
                        SetRelativeBase(parameters);
                        Debug.Write(_relativeBase);
                    }
                    break;
                    case 99:
                    {
                        Debug.WriteLine($"HLT!");
                        Halted = true;
                        return _output;
                    }
                    default:
                        throw new Exception($"Opcode error: {intCode}");
                }

                Debug.WriteLine("");
            }

            throw new System.Exception("Program finished unexpectedly.");
        }

        private long[] ReadNextParameters(ParameterMode[] modes, string io, int parameterCount)
        {
            var values = new long[parameterCount];

            for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                long intCode = ReadNext();

                if (io[parameterIndex] == 'R')
                {
                    switch (modes[parameterIndex])
                    {
                        case ParameterMode.PositionMode:
                        {
                            values[parameterIndex] = _opcodes[intCode];
                        }
                        break;
                        case ParameterMode.ImmediateMode:
                        {
                            values[parameterIndex] = intCode;
                        }
                        break;
                        case ParameterMode.RelativeMode:
                        {
                            values[parameterIndex] = _opcodes[intCode + _relativeBase];
                        }
                        break;
                        default:
                            throw new Exception("Unknown ParameterMode.");
                    }
                }
                else if (io[parameterIndex] == 'W')
                {
                    switch (modes[parameterIndex])
                    {
                        case ParameterMode.PositionMode:
                        {
                            values[parameterIndex] = intCode;
                        }
                        break;
                        case ParameterMode.ImmediateMode:
                        {
                            throw new Exception("\"Parameters that an instruction writes to will never be in immediate mode.\"");
                        }
                        case ParameterMode.RelativeMode:
                        {
                            values[parameterIndex] = intCode + _relativeBase;
                        }
                        break;
                        default:
                            throw new Exception("Unknown ParameterMode.");
                    }
                }
            }

            return values;
        }

        private ParameterMode[] ParameterModes(long opcode)
        {
            string opcodeString = opcode.ToString();

            var modes = new ParameterMode[3];

            if (opcodeString.Length >= 5)
            {
                modes[2] = (ParameterMode)Enum.Parse(typeof(ParameterMode), opcodeString.Substring(opcodeString.Length - 5, 1));
            }
            if (opcodeString.Length >= 4)
            {
                modes[1] = (ParameterMode)Enum.Parse(typeof(ParameterMode), opcodeString.Substring(opcodeString.Length - 4, 1));
            }
            if (opcodeString.Length >= 3)
            {
                modes[0] = (ParameterMode)Enum.Parse(typeof(ParameterMode), opcodeString.Substring(opcodeString.Length - 3, 1));
            }

            return modes;
        }

        private void Addition(long[] parameters)
        {
            _opcodes[parameters[2]] = parameters[0] + parameters[1];
        }

        private void Multiplication(long[] parameters)
        {
            _opcodes[(int)parameters[2]] = parameters[0] * parameters[1];
        }

        private void Input(long[] parameters)
        {
            _opcodes[(int)parameters[0]] = AskForInput();
        }

        private void Output(long[] parameters)
        {
            _output = parameters[0];
        }

        private void JumpIfTrue(long[] parameters)
        {
            if (parameters[0] != 0)
            {
                _playhead = (int)parameters[1];
            }
        }

        private void JumpIfFalse(long[] parameters)
        {
            if (parameters[0] == 0)
            {
                _playhead = (int)parameters[1];
            }
        }

        private void LessThan(long[] parameters)
        {
            _opcodes[(int)parameters[2]] = parameters[0] < parameters[1] ? 1 : 0;
        }

        private void Equal(long[] parameters)
        {
            _opcodes[(int)parameters[2]] = parameters[0] == parameters[1] ? 1 : 0;
        }

        private void SetRelativeBase(long[] parameters)
        {
            _relativeBase += parameters[0];
        }
    }
}
