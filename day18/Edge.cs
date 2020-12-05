using System;
using System.Collections.Generic;
using System.Text;

namespace day18
{
    public struct Edge
    {
        public string Name;
        public char[] Doors;
        public int Cost;

        public Edge(string name, RetraceData retraceData)
        {
            Name = name;
            Doors = retraceData.Doors.ToArray();
            Cost = retraceData.Length;
        }

        public static string EdgeName(Node a, Node b)
        {
            if (a.Name > b.Name)
                return $"{a.Name}{b.Name}";
            return $"{b.Name}{a.Name}";
        }
    }
}
