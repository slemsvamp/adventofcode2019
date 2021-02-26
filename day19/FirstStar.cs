using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace day19
{
    public class FirstStar
    {
        private static int _index = 0;
        private static bool _askForX = true;

        public static string Run(List<long> opcodes)
        {
            var runner = new OpcodeRunner(4000, opcodes, AskForInput);

            while (_index < 100)
            {
                runner.SetPlayhead(0);
                var output = runner.Run();

                Console.Write(output.ToString());
            }

            return "";
        }

        public static long AskForInput()
        {
            long current = -1;

            if (_askForX)
                current = _index % 10;
            else
            {
                current = _index / 10;
                _index++;
            }
            
            _askForX = !_askForX;
            return current;
        }
    }
}