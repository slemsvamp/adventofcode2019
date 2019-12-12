using System;
using System.Collections.Generic;
using System.Text;

namespace day12
{
    public class Vector3
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public static Vector3 operator +(Vector3 target, Vector3 modifier)
        {
            return new Vector3
            {
                X = target.X + modifier.X,
                Y = target.Y + modifier.Y,
                Z = target.Z + modifier.Z
            };
        }

        public static bool operator ==(Vector3 source, Vector3 target)
        {
            return source.X == target.X && source.Y == target.Y && source.Z == target.Z;
        }

        public static bool operator !=(Vector3 source, Vector3 target)
        {
            return !(source == target);
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3 { X = 0, Y = 0, Z = 0 };
            }
        }
    }
}
