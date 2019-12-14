using System;
using System.Collections.Generic;
using System.Linq;

namespace day14
{
    public class SecondStar
    {
        public static string Run()
        {
            return new ChemCooker("input.txt").FuelForOre(1_000_000_000_000).ToString();
        }
    }
}
