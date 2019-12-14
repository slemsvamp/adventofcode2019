using System;
using System.Collections.Generic;
using System.Linq;

namespace day14
{
    public class FirstStar
    {
        public static string Run()
        {
            return new ChemCooker("input.txt").OrePerFuel(1).ToString();
        }
    }
}
