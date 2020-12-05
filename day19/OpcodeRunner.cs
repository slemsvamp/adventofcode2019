using System;
using System.Collections.Generic;

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

                switch (instruction)
                {
                    case 1:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Addition(parameters);
                    }
                    break;
                    case 2:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Multiplication(parameters);
                    }
                    break;
                    case 3:
                    {
                        var parameters = ReadNextParameters(parameterModes, "W", 1);
                        Input(parameters);
                    }
                    break;
                    case 4:
                    {
                        var parameters = ReadNextParameters(parameterModes, "R", 1);
                        Output(parameters);
                    }
                    return _output;
                    case 5:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RR", 2);
                        JumpIfTrue(parameters);
                    }
                    break;
                    case 6:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RR", 2);
                        JumpIfFalse(parameters);
                    }
                    break;
                    case 7:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        LessThan(parameters);
                    }
                    break;
                    case 8:
                    {
                        var parameters = ReadNextParameters(parameterModes, "RRW", 3);
                        Equal(parameters);
                    }
                    break;
                    case 9:
                    {
                        var parameters = ReadNextParameters(parameterModes, "R", 1);
                        SetRelativeBase(parameters);
                    }
                    break;
                    case 99:
                    {
                        Halted = true;
                        return _output;
                    }
                    default:
                    throw new Exception($"Opcode error: {intCode}");
                }
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
