using System;
using System.Collections.Generic;
using System.Threading;

namespace day12
{
    public class SecondStar
    {
        private static List<Moon> _moons;

        public static string Run(List<Vector3> coordinates)
        {
            _moons = new List<Moon>();

            foreach (var coordinate in coordinates)
            {
                _moons.Add(new Moon
                {
                    Position = coordinate,
                    Velocity = Vector3.Zero
                });
            }

            int counter = 0;

            var energySignatures = new Dictionary<long, List<int>>();

            int moonIndex = 3;

            // 0 = 882 // 462
            // 1 = 503 // 230
            // 2 = 195 // 308
            // 3 = 242 // 261

            // the problem is probably solved by noting down the
            // periodicity of which a moon comes back to its original position
            // 

            // record every time a moon has the same energy as it had on step 0

            

            long startingEnergy = 0;
            foreach (var moon in _moons)
            {
                startingEnergy += CalculateEnergy(moon);
            }

            energySignatures.Add(startingEnergy, new List<int> { 0 });

            while (true)
            {
                var energy = OneStep(_moons);

                var moonIndexZeroEnergy = CalculateEnergy(_moons[moonIndex]);
                energy = moonIndexZeroEnergy;

                counter++;

                if (energy == startingEnergy)
                {
                    break;
                }

                if (!energySignatures.ContainsKey(energy))
                {
                    energySignatures.Add(energy, new List<int> { counter });
                }
                else
                {
                    if (OneMoonOrbit(energySignatures[energy], _moons[moonIndex].Position, moonIndex))
                    {
                        Console.WriteLine($"{counter}");
                    }
                    //if (ReplayAndMatch(energySignatures[energy], coordinates))
                    //{
                    //    break;
                    //}
                    else
                    {
                        energySignatures[energy].Add(counter);
                    }
                }

                if (counter > 2772)
                {
                    break;
                }
            }

            return "";
        }

        private static bool ReplayAndMatch(List<int> reproduceSteps, List<Vector3> coordinates)
        {
            foreach (var reproduce in reproduceSteps)
            {
                var moons = new List<Moon>();

                foreach (var coordinate in coordinates)
                {
                    moons.Add(new Moon
                    {
                        Position = coordinate,
                        Velocity = Vector3.Zero
                    });
                }

                int steps = reproduce;

                while (steps > 0)
                {
                    OneStep(moons);

                    int matches = 0;

                    for (int moonIndex = 0; moonIndex < moons.Count; moonIndex++)
                    {
                        if (moons[moonIndex].Position == _moons[moonIndex].Position &&
                            moons[moonIndex].Velocity == _moons[moonIndex].Velocity)
                        {
                            matches++;
                        }
                    }

                    if (matches == moons.Count)
                    {
                        return true;
                    }

                    steps--;
                }
            }

            return false;
        }

        private static bool OneMoonOrbit(List<int> reproduceSteps, Vector3 coordinate, int moonIndex)
        {
            foreach (var reproduce in reproduceSteps)
            {
                var moon = new Moon
                {
                    Position = coordinate,
                    Velocity = Vector3.Zero
                };

                var moons = new List<Moon> { moon };

                int steps = reproduce;

                while (steps > 0)
                {
                    OneStep(moons);

                    if (moons[0].Position == _moons[moonIndex].Position &&
                            moons[0].Velocity == _moons[moonIndex].Velocity)
                    {
                        return true;
                    }

                    steps--;
                }
            }

            return false;
        }

        private static long OneStep(List<Moon> moons)
        {
            long totalEnergy = 0;

            foreach (var moon in moons)
            {
                ApplyGravity(moon);
            }

            foreach (var moon in moons)
            {
                Update(moon);
            }

            foreach (var moon in moons)
            {
                var moonEnergy = CalculateEnergy(moon);

                totalEnergy += moonEnergy;
            }

            return totalEnergy;
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
