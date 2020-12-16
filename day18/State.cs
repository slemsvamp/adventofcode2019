using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace day18
{
    public struct State
    {
        public HashSet<char> Keys;
        public Robot[] Robots;

        public State FromHash(long hash)
        {
            var state = new State
            {
                Robots = new Robot[]
                {
                    new Robot
                    {
                        Position = new Point
                        {
                            X = 0,
                            Y = 0
                        }
                    }
                },
                Keys = new HashSet<char>()
            };

            return state;
        }

        public long ToHash()
        {
            long hash = Robots[0].Position.X;
            hash += 7 ^ 2 * Robots[0].Position.Y;
            hash += 14 ^ 2 * Robots[0].Distance;

            hash += 26 ^ 2 * Robots[1].Position.X;
            hash += 33 ^ 2 * Robots[1].Position.Y;
            hash += 40 ^ 2 * Robots[1].Distance;

            hash += 52 ^ 2 * Robots[2].Position.X;
            
            long hash2 = Robots[2].Position.Y;
            hash2 += 7 ^ 2 * Robots[2].Distance;

            hash2 += 19 ^ 2 * Robots[3].Position.X;
            hash2 += 26 ^ 2 * Robots[3].Position.Y;
            hash2 += 33 ^ 2 * Robots[3].Distance;

            var keys = Keys.OrderBy(k => k).ToArray();

            for (int index = 0; index < keys.Length; index++)
            {
                int bit = keys[index] - 'a';
                hash += 40 ^ 2 * bit;
            }




            return hash;
        }
    }
}
