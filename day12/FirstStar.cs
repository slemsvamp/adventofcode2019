using System;
using System.Collections.Generic;

namespace day12
{
    public class FirstStar
    {
        private static List<Moon> _moons;

        public static string Run(List<Vector3> coordinates)
        {
            // 601 is wrong
            // 14645 is wrong

            _moons = new List<Moon>();

            foreach (var coordinate in coordinates)
            {
                _moons.Add(new Moon
                {
                    Position = coordinate,
                    Velocity = Vector3.Zero
                });
            }

            long totalEnergy = 0;
            int steps = 1000;

            int counter = 0;

            //foreach (var moon in _moons)
            //{
            //    PrintMoonPosAndVel(moon);
            //}

            //Console.WriteLine($"Total Energy: {totalEnergy}");
            //Console.ReadKey(true);

            while (steps > 0)
            {
                totalEnergy = 0;

                foreach (var moon in _moons)
                {
                    ApplyGravity(moon);
                }

                foreach (var moon in _moons)
                {
                    Update(moon);
                }

                foreach (var moon in _moons)
                {
                    var moonEnergy = CalculateEnergy(moon);

                    totalEnergy += moonEnergy;
                }

                steps--;
                counter++;

                //if (counter % 10 == 0)
                //{
                //    foreach (var moon in _moons)
                //    {
                //        PrintMoonPosAndVel(moon);
                //    }

                //    Console.WriteLine($"Total Energy: {totalEnergy}");
                //    Console.ReadKey(true);
                //}
            }

            //Console.WriteLine("Done.");
            //Console.ReadKey(true);

            return totalEnergy.ToString();
        }

        private static void PrintMoonPosAndVel(Moon moon)
        {
            Console.WriteLine($"pos=<x= {moon.Position.X}, y= {moon.Position.Y}, z= {moon.Position.Z}>, vel=<x= {moon.Velocity.X}, y= {moon.Velocity.Y}, z= {moon.Velocity.Z}>");
        }

        private static long CalculateEnergy(Moon moon)
        {
            var potentialEnergy = Math.Abs(moon.Position.X) + Math.Abs(moon.Position.Y) + Math.Abs(moon.Position.Z);
            var kineticEnergy = Math.Abs(moon.Velocity.X) + Math.Abs(moon.Velocity.Y) + Math.Abs(moon.Velocity.Z);
            return potentialEnergy * kineticEnergy;
        }

        public static void ApplyGravity(Moon moon)
        {
            foreach (var otherMoon in _moons)
            {
                if (moon == otherMoon)
                {
                    continue;
                }

                int x = moon.Position.X < otherMoon.Position.X ? 1 : moon.Position.X == otherMoon.Position.X ? 0 : -1;
                int y = moon.Position.Y < otherMoon.Position.Y ? 1 : moon.Position.Y == otherMoon.Position.Y ? 0 : -1;
                int z = moon.Position.Z < otherMoon.Position.Z ? 1 : moon.Position.Z == otherMoon.Position.Z ? 0 : -1;

                var modificationVector = new Vector3
                {
                    X = x,
                    Y = y,
                    Z = z
                };

                moon.Velocity += modificationVector;
            }
        }

        public static void Update(Moon moon)
        {
            moon.Position += moon.Velocity;
        }
    }
}
