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
            // 12'937'777'105'696, too low
            var periodicities = new List<(Vector3, Vector3)>();

            for (int moonIndex = 0; moonIndex < coordinates.Count; moonIndex++)
            {
                (Vector3 periodicity, Vector3 starts) = CalculatePeriodicity(coordinates, moonIndex);

                periodicities.Add((periodicity, starts));
            }

            int c = 0;
            var sums = new List<long>();
            foreach (var px in periodicities)
            {
                var p = px.Item1;
                var x = px.Item2;
                c++;
                Console.WriteLine($"{c}. X={p.X}, Y={p.Y}, Z={p.Z}, Sum={p.X + p.Y + p.Z}, Starts X={x.X}, Y={x.Y}, Z={x.Z}");
                sums.Add(p.X + p.Y + p.Z);
            }

            


            return string.Empty;
        }

        public static (Vector3, Vector3) CalculatePeriodicity(List<Vector3> coordinates, int moonIndex)
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

            var turnsRepeated = new List<long>();
            long turn = 0;
            long periodicX = -1, periodicY = -1, periodicZ = -1;
            long periodicXStart = -1, periodicYStart = -1, periodicZStart = -1;
            bool lockX = false, lockY = false, lockZ = false;
            var periodics = new List<Vector3>();

            while (true)
            {
                OneStep(moons);
                turn++;

                if (!lockX && CheckX(moons[moonIndex], coordinates[moonIndex]))
                {
                    var diffX = turn - periodicX;
                    if (periodicX > 0 && diffX == periodicX)
                    {
                        lockX = true;
                    }

                    if (!lockX)
                    {
                        periodicXStart = turn;
                    }

                    periodicX = periodicX > 0 ? diffX : turn;
                }

                if (!lockY && CheckY(moons[moonIndex], coordinates[moonIndex]))
                {
                    var diffY = turn - periodicY;
                    if (periodicY > 0 && diffY == periodicY)
                    {
                        lockY = true;
                    }

                    if (!lockY)
                    {
                        periodicYStart = turn;
                    }

                    periodicY = periodicY > 0 ? diffY : turn;
                }

                if (!lockZ && CheckZ(moons[moonIndex], coordinates[moonIndex]))
                {
                    var diffZ = turn - periodicZ;

                    if (periodicZ > 0 && diffZ == periodicZ)
                    {
                        lockZ = true;
                    }

                    if (!lockZ)
                    {
                        periodicZStart = turn;
                    }

                    periodicZ = periodicZ > 0 ? diffZ : turn;
                }

                if (lockX && lockY && lockZ)
                {
                    return (new Vector3 { X = periodicX, Y = periodicY, Z = periodicZ }, new Vector3 { X = periodicXStart, Y = periodicYStart, Z = periodicZStart });
                }

                //    if (moons[moonIndex].Position == coordinates[moonIndex] && moons[moonIndex].Velocity == Vector3.Zero)
                //{
                //    var diff = turnsRepeated.Count > 0 ? turn - turnsRepeated[turnsRepeated.Count - 1] : turn;

                //    if (diff >= 0)
                //    {
                //        if (diff == turn)
                //        {
                //            return diff;
                //        }

                //        turnsRepeated.Add(diff);
                //    }
                //}
            }
        }

        public static bool CheckX(Moon moon, Vector3 coordinate)
        {
            if (moon.Position.X == coordinate.X && moon.Velocity.X == 0) return true;
            return false;
        }
        public static bool CheckY(Moon moon, Vector3 coordinate)
        {
            if (moon.Position.Y == coordinate.Y && moon.Velocity.Y == 0) return true;
            return false;
        }
        public static bool CheckZ(Moon moon, Vector3 coordinate)
        {
            if (moon.Position.Z == coordinate.Z && moon.Velocity.Z == 0) return true;
            return false;
        }

        public static string Run2(List<Vector3> coordinates)
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
                OneStep(_moons);
                var energy = CalculateTotalEnergy(_moons);

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

        private static void OneStep(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                ApplyGravity(moon, moons);
            }

            foreach (var moon in moons)
            {
                Update(moon);
            }
        }

        private static long CalculateTotalEnergy(List<Moon> moons)
        {
            long totalEnergy = 0;

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

        public static void ApplyGravity(Moon moon, List<Moon> otherMoons)
        {
            foreach (var otherMoon in otherMoons)
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
