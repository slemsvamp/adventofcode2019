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
        public int Distance;

        //public void FromHash(string hash)
        //{
        //    var bytes = Convert.FromBase64String(hash);

        //    long hashA = 0, hashB = 0;

        //    hashA = BitConverter.ToInt64(bytes.Take(8).ToArray());
        //    hashB = BitConverter.ToInt64(bytes.Skip(8).Take(8).ToArray());

        //    Robots = new Robot[]
        //    {
        //            new Robot
        //            {
        //                Position = new Point
        //                {
        //                    X = (int)(hashA % 128),
        //                    Y = (int)(hashA / Math.Pow(2, 7) % 128)
        //                },
        //                Distance = (int)(hashA / Math.Pow(2, 14) % 4096)
        //            },
        //            new Robot
        //            {
        //                Position = new Point
        //                {
        //                    X = (int)(hashA / Math.Pow(2, 26) % 128),
        //                    Y = (int)(hashA / Math.Pow(2, 33) % 128)
        //                },
        //                Distance = (int)(hashA / Math.Pow(2, 40) % 4096)
        //            },
        //            new Robot
        //            {
        //                Position = new Point
        //                {
        //                    X = (int)(hashA / Math.Pow(2, 52) % 128),
        //                    Y = (int)(hashB % 128)
        //                },
        //                Distance = (int)(hashB / Math.Pow(2, 7) % 4096)
        //            },
        //            new Robot
        //            {
        //                Position = new Point
        //                {
        //                    X = (int)(hashB / Math.Pow(2, 19) % 128),
        //                    Y = (int)(hashB / Math.Pow(2, 26) % 128)
        //                },
        //                Distance = (int)(hashB / Math.Pow(2, 33) % 4096)
        //            }
        //    };

        //    Keys = new HashSet<char>(bytes.Skip(16).Select(k => (char)k));
        //}

        public string ToHash()
        {
            long hashA = 0;

            if (Robots != null)
            {
                hashA += Robots[0].Position.X;
                hashA += (long)Math.Pow(2, 7) * Robots[0].Position.Y;

                hashA += (long)Math.Pow(2, 14) * Robots[1].Position.X;
                hashA += (long)Math.Pow(2, 21) * Robots[1].Position.Y;

                hashA += (long)Math.Pow(2, 28) * Robots[2].Position.X;
                hashA += (long)Math.Pow(2, 35) * Robots[2].Position.Y;

                hashA += (long)Math.Pow(2, 42) * Robots[3].Position.X;
                hashA += (long)Math.Pow(2, 49) * Robots[3].Position.Y;
            }

            var keys = new byte[0];

            if (Keys != null)
                keys = Keys.OrderBy(k => k).Select(k => (byte)k).ToArray();

            var bytes = BitConverter.GetBytes(hashA).Concat(keys).ToArray();

            return Convert.ToBase64String(bytes);
        }

        public string ToOldHash()
        {
            long hashA = 0;
            long hashB = 0;

            if (Robots != null)
            {
                hashA += Robots[0].Position.X;
                hashA += (long)Math.Pow(2, 7) * Robots[0].Position.Y;
                hashA += (long)Math.Pow(2, 14) * Robots[0].Distance;

                hashA += (long)Math.Pow(2, 26) * Robots[1].Position.X;
                hashA += (long)Math.Pow(2, 33) * Robots[1].Position.Y;
                hashA += (long)Math.Pow(2, 40) * Robots[1].Distance;

                hashA += (long)Math.Pow(2, 52) * Robots[2].Position.X;
                hashB += Robots[2].Position.Y;
                hashB += (long)Math.Pow(2, 7) * Robots[2].Distance;

                hashB += (long)Math.Pow(2, 19) * Robots[3].Position.X;
                hashB += (long)Math.Pow(2, 26) * Robots[3].Position.Y;
                hashB += (long)Math.Pow(2, 33) * Robots[3].Distance;
            }

            var keys = new byte[0];

            if (Keys != null)
                keys = Keys.OrderBy(k => k).Select(k => (byte)k).ToArray();

            var bytesA = BitConverter.GetBytes(hashA);
            var bytesB = BitConverter.GetBytes(hashB);

            var bytes = bytesA.Concat(bytesB).Concat(keys).ToArray();

            return Convert.ToBase64String(bytes);
        }
    }
}
