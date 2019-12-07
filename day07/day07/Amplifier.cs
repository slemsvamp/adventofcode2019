using System;
using System.Collections.Generic;
using System.Text;

namespace day07
{
    public class Amplifier
    {
        public Amplifier Next;
        public int InputSignal;

        private OpcodeRunner _opcodeRunner;

        private List<int> _opcodes;

        public List<int> Opcodes
        {
            get
            {
                return new List<int>(_opcodes);
            }
        }

        public bool Halted { get; internal set; }
        public int Output { get; internal set; }

        public Amplifier(List<int> opcodes)
        {
            _opcodes = new List<int>(opcodes);
            _opcodeRunner = new OpcodeRunner(opcodes);
        }

        public void SetOpcodes(List<int> opcodes)
        {
            _opcodeRunner.SetOpcodes(opcodes);
        }

        public void Reset()
        {
            _opcodeRunner.SetOpcodes(_opcodes);
        }

        public void AddInput(int input)
        {
            _opcodeRunner.AddInput(input);
        }

        public int Run()
        {
            var value = _opcodeRunner.Run();

            Output = value;
            Halted = _opcodeRunner.Halted;

            return value;
        }
    }
}
