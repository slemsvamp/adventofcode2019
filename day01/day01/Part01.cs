using System.Collections.Generic;
using System.Linq;

namespace day01
{
    public class Part01
    {
        public string Run(List<int> moduleMasses)
        {
            return moduleMasses.Sum(mass => Calculations.MassToFuel(mass)).ToString();
        }
    }
}
