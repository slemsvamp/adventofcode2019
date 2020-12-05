using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day18
{
    public class InputParser
    {
        public static string[] Parse(string filename)
        {
            return File.ReadAllLines(filename);
        }
    }
}
