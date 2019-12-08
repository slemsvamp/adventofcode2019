using System.Collections.Generic;
using System.Linq;

namespace day01
{
    public class Part02
    {
        public string Run(List<int> moduleMasses)
        {
            return moduleMasses.Sum(mass => Calculations.FuelForFuel(mass)).ToString();
        }
    }
}