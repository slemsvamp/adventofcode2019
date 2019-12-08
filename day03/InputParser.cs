using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static day03.Program;

namespace day03
{
    public class InputParser
    {
        public static List<Wire> Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var wires = new List<Wire>();

            int wireIndex = 0;

            foreach (var line in lines)
            {
                var orders = new List<Order>();
                var orderItems = line.Split(new[] { "," }, StringSplitOptions.None);

                Array.ForEach(orderItems, i => orders.Add(new Order { Letter = i.Substring(0, 1), Number = int.Parse(i.Substring(1)) }));

                wires.Add(new Wire
                {
                    Index = wireIndex++,
                    Orders = orders
                });
            }

            return wires;
        }
    }
}
