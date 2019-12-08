using System;
using System.Collections.Generic;
using System.Text;

namespace day01
{
    public class Calculations
    {
        public static int MassToFuel(int mass)
        {
            return mass / 3 - 2;
        }

        public static int FuelForFuel(int mass)
        {
            int total = 0;
            while (mass > 0)
            {
                int fuel = MassToFuel(mass);
                if (fuel <= 0)
                {
                    break;
                }

                mass = fuel;
                total += fuel;
            }
            return total;
        }
    }
}