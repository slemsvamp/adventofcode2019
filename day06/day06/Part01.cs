using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace day06
{
    public class Part01
    {
        public class StellarObject
        {
            public string Name;
            public StellarObject OrbitsAround;
            public List<StellarObject> OrbittedBy;

            public StellarObject(string name)
            {
                Name = name;
                OrbittedBy = new List<StellarObject>();
            }
        }

        public string Run()
        {
            var lines = File.ReadAllLines("input.txt");
            var stellarObjects = new Dictionary<string, StellarObject>();

            foreach (var line in lines)
            {
                string[] parts = line.Split(new[] { ")" }, StringSplitOptions.None);

                if (!stellarObjects.ContainsKey(parts[0]))
                {
                    stellarObjects.Add(parts[0], new StellarObject(parts[0]));
                }

                var stellarObject = stellarObjects[parts[0]];

                if (!stellarObjects.ContainsKey(parts[1]))
                {
                    stellarObjects.Add(parts[1], new StellarObject(parts[1]));
                }

                var orbitter = stellarObjects[parts[1]];

                stellarObject.OrbittedBy.Add(orbitter);

                orbitter.OrbitsAround = stellarObject;
            }

            var stats = new Dictionary<string, StellarObjectStats>();

            foreach (var nonOrbitter in stellarObjects.Values.Where(s => s.OrbitsAround == null))
            {
                int orbitCount = OrbitCount(nonOrbitter, stats);
            }

            StellarObjectStats general = new StellarObjectStats();

            foreach (var stat in stats)
            {
                general.DirectOrbits += stat.Value.DirectOrbits;
                general.IndirectOrbits += stat.Value.IndirectOrbits;
            }

            return (general.DirectOrbits + general.IndirectOrbits).ToString();
        }

        public int OrbitCount(StellarObject stellarObject, Dictionary<string, StellarObjectStats> stats)
        {
            stats.Add(stellarObject.Name, new StellarObjectStats());

            stats[stellarObject.Name].DirectOrbits = stellarObject.OrbittedBy.Count();

            int orbitCount = 0;

            foreach (var orbitter in stellarObject.OrbittedBy)
            {
                orbitCount += OrbitCount(orbitter, stats);
            }

            stats[stellarObject.Name].IndirectOrbits = orbitCount;

            return stellarObject.OrbittedBy.Count() + orbitCount;
        }

        public class StellarObjectStats
        {
            public int DirectOrbits;
            public int IndirectOrbits;
        }
    }
}
