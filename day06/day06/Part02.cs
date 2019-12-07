using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace day06
{
    public class Part02
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
            // 441 is not right

            var lines = File.ReadAllLines("input.txt");
            var stellarObjects = new Dictionary<string, StellarObject>();

            var youLineage = new List<StellarObject>();
            var sanLineage = new List<StellarObject>();

            StellarObject lastYouDescendant = null;
            StellarObject lastSanDescendant = null;

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

                if (orbitter.Name == "YOU")
                {
                    lastYouDescendant = orbitter;
                }

                if (orbitter.Name == "SAN")
                {
                    lastSanDescendant = orbitter;
                }

                if (orbitter == lastYouDescendant)
                {
                    lastYouDescendant = stellarObject;
                    youLineage.Add(stellarObject);

                    var ancestor = stellarObject.OrbitsAround;
                    while (ancestor != null)
                    {
                        youLineage.Add(ancestor);
                        lastYouDescendant = ancestor;
                        ancestor = ancestor.OrbitsAround;
                    }
                }

                if (orbitter == lastSanDescendant)
                {
                    lastSanDescendant = stellarObject;
                    sanLineage.Add(stellarObject);

                    var ancestor = stellarObject.OrbitsAround;
                    while (ancestor != null)
                    {
                        sanLineage.Add(ancestor);
                        lastSanDescendant = ancestor;
                        ancestor = ancestor.OrbitsAround;
                    }
                }
            }

            var commonAncestors = new List<StellarObject>();

            foreach (var so in youLineage)
            {
                if (sanLineage.Contains(so))
                {
                    commonAncestors.Add(so);
                }
            }


            var you = stellarObjects["YOU"];

            var san = stellarObjects["SAN"];

            StellarObject parent = you.OrbitsAround;
            int lineage = 0;

            while (parent != null)
            {
                if (commonAncestors.Contains(parent)) break;
                lineage++;

                parent = parent.OrbitsAround;
            }
            //var lineage = LineageCount(parent, san);

            //if (lineage > 0)
            //{
            //    Console.WriteLine($"SAN has {parent.Name} as common Ancestor, lineage = {lineage}");
            //    break;
            //}

            //parent = parent.OrbitsAround;

            var totalLineage = lineage;

            lineage = 0;
            parent = san.OrbitsAround;
            while (parent != null)
            {
                if (commonAncestors.Contains(parent)) break;
                lineage++;

                parent = parent.OrbitsAround;
            }

            totalLineage += lineage;

            return totalLineage.ToString();
        }

        public int LineageCount(StellarObject parent, StellarObject child)
        {
            StellarObject possibleParent = child.OrbitsAround;
            int ancestors = 0;

            while (possibleParent != null || possibleParent == parent)
            {
                ancestors++;
                possibleParent = child.OrbitsAround;
            }

            return possibleParent == parent ? ancestors : -1;
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
