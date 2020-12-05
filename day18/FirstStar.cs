using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace day18
{
    public class FirstStar
    {
        private static Dictionary<char, Point> _keyPositions { get; set; }
        private static Dictionary<char, Point> _doorPositions { get; set; }
        private static Point _characterPosition { get; set; }
        private static List<char> _collectedKeys { get; set; }

        private volatile static int _bestLength;
        private volatile static int _finishedThreads;

        public static string Run()
        {
            var map = InputParser.Parse("input.txt");
            var size = new Size(map[0].Length, map.Length);

            _keyPositions = new Dictionary<char, Point>();
            _doorPositions = new Dictionary<char, Point>();
            _bestLength = int.MaxValue;

            var keyRegex = new Regex("^[a-z]$");
            var doorRegex = new Regex("^[A-Z]$");

            for (int y = 0; y < size.Height; y++)
            {
                var row = map[y];
                for (int x = 0; x < size.Width; x++)
                {
                    var point = new Point(x, y);
                    var character = row[x];
                    if (character == '@')
                        _characterPosition = point;
                    else if (keyRegex.Match(character.ToString()).Success)
                        _keyPositions.Add(character, point);
                    else if (doorRegex.Match(character.ToString()).Success)
                        _doorPositions.Add(character, point);
                }
            }

            var grid = new Grid(map, size);
            var retraces = grid.GenerateRetraces(_keyPositions, _characterPosition);

            var graph = new Graph(retraces, _keyPositions, _characterPosition);

            graph.StartTraversal('@', new HashSet<char>(_keyPositions.Keys));

            return string.Empty;
        }


#if oldcodeisenabled
        public static string Run2()
        {
            // 5876 is too high
            // 6382 is too high

            var map = InputParser.Parse("input.txt");
            var width = map.IndexOf('\r');

            map = map.Replace("\r\n", "");

            _keyPositions = new Dictionary<char, Point>();
            _doorPositions = new Dictionary<char, Point>();
            _bestLength = int.MaxValue;

            for (int index = 0; index < map.Length; index++)
            {
                var character = map[index];

                switch (character)
                {
                    case '@':
                        _characterPosition = new Point(index % width, index / width);
                        continue;
                    case '#':
                    case '.':
                        continue;
                    default:
                        if (character.ToString() == character.ToString().ToUpper())
                        {
                            _doorPositions.Add(character, new Point(index % width, index / width));
                        }
                        else
                        {
                            _keyPositions.Add(character, new Point(index % width, index / width));
                        }
                        continue;
                }
            }

            var grid = new Grid(new Size(width, map.Length / width))
            {
                Map = map.ToCharArray()
            };

#if DEBUG
            //DrawGrid(grid);
#endif
            //var keyList = new List<(char, Point)>();
            //foreach (var kvp in _keyPositions)
            //{
            //    keyList.Add((kvp.Key, kvp.Value));
            //}

            //var pathfinder = new Pathfinder();
            //var retraceData = new List<RetraceData>();

            //for (int fromIndex = 0; fromIndex < keyList.Count; fromIndex++)
            //{
            //    for (int toIndex = 0; toIndex < keyList.Count; toIndex++)
            //    {
            //        if (fromIndex == toIndex)
            //            continue;

            //        var retrace = pathfinder.FindPath(keyList[fromIndex].Item2, keyList[toIndex].Item2, grid);

            //        retraceData.Add(new RetraceData
            //        {
            //            From = keyList[fromIndex].Item1,
            //            To = keyList[toIndex].Item1,
            //            Retrace = retrace
            //        });
            //    }
            //}

            var allPossibleRetraces = GetAllPossibleRetraces(grid).ToList();

            var characterNode = new Node { Name = '@', Point = _characterPosition };
            var nodeList = new List<Node>();
            foreach (var keyValuePair in _keyPositions)
                nodeList.Add(new Node { Name = keyValuePair.Key, Point = keyValuePair.Value });
            nodeList.Add(characterNode);

            var dijkstra = new Dijkstra(nodeList, characterNode, (n) =>
            {
                return new Edge[0];
            });

            var availableStarterPaths = allPossibleRetraces.Where(r => r.From == '@' && r.Doors.Count == 0).ToList();

            var paths = new List<Path>();

            //availableStarterPaths.ForEach(r => paths.Add(new Path
            //{
            //    Length = r.Length,
            //    CollectedKeys = new List<char> { r.To },
            //    LastPosition = r.To,
            //    PathCount = 1,
            //    Depth = 0
            //}));

            //long highestPathCount = 0;
            //int bestLength = 5876;
            //int cancelled = 0;
            //Path shortestPath = null;

            var threads = new Thread[availableStarterPaths.Count];
            var best = new int[availableStarterPaths.Count];

            for (int threadIndex = 0; threadIndex < availableStarterPaths.Count; threadIndex++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(threadNumber =>
                {
                    var r = availableStarterPaths[(int)threadNumber];
                    var bestForPath = RunStarterPath(new List<Path>
                    {
                        new Path
                        {
                            Length = r.Length,
                            CollectedKeys = new List<char> { r.To },
                            LastPosition = r.To,
                            Depth = 0
                        }
                    }, allPossibleRetraces);
                    best[threadIndex] = bestForPath;
                    _finishedThreads++;
                }));

                threads[threadIndex] = thread;
            }

            for (int threadIndex = 0; threadIndex < threads.Length; threadIndex++)
                threads[threadIndex].Start(threadIndex);

            while (_finishedThreads < availableStarterPaths.Count)
            { }

            //var shortestLength = closedPaths.Where(p => p.CollectedKeys.Count == _keyPositions.Count).Min(p => p.Length);

            //var shortestPath = closedPaths.Where(p => p.CollectedKeys.Count == _keyPositions.Count && p.Length == shortestLength).First();

            //do
            //{
            //    DrawGrid(grid);
            //    DrawPath(grid, shortestPath);
            //    Thread.Sleep(40);
            //    //Console.ReadKey(true);
            //    shortestPath = shortestPath.Previous;
            //} while (shortestPath.Previous != null);

            return best.Min().ToString();
        }

        public static int RunStarterPath(List<Path> paths, List<RetraceData> allPossibleRetraces)
        {
            Func<char, List<char>, bool> needsKey = new Func<char, List<char>, bool>((to, collectedKeys) =>
            {
                return !collectedKeys.Contains(to);
            });

            Func<RetraceData, List<char>, bool> hasKeyForTheDoors = new Func<RetraceData, List<char>, bool>((retrace, collectedKeys) =>
            {
                foreach (var door in retrace.Doors)
                {
                    if (!collectedKeys.Contains(door))
                    {
                        return false;
                    }
                }
                return true;
            });

            while (paths.Count > 0)
            {
                // remember, we need to pick the shortest path all the time to work on
                //var minPathLength = ;
                var currentPath = paths.OrderByDescending(p => p.Depth).ThenBy(p => p.Length).First();

                paths.Remove(currentPath);

                if (currentPath.Length > _bestLength)
                {
                    //cancelled++;

                    //if (cancelled % 100_000 == 0)
                    //{
                    //    Console.WriteLine();
                    //    Console.WriteLine($"Cancelled {cancelled}.");
                    //}

                    continue;
                }

                //if (currentPath.CollectedKeys.Count == _keyPositions.Count)
                //{
                //    allKeysCollected = true;
                //    shortestPath = currentPath;
                //    break;
                //}

                var possibleDestinations = allPossibleRetraces
                        .Where(r => r.From == currentPath.LastPosition && HasKeyForTheDoors(r, currentPath.CollectedKeys) && NeedsKey(r.To, currentPath.CollectedKeys))
                        .ToArray();

                if (possibleDestinations.Length == 0)
                {
                    if (currentPath.CollectedKeys.Count != _keyPositions.Count)
                        throw new Exception();

                    Console.Write("$");

                    //if (highestPathCount < currentPath.PathCount)
                    //{
                    //    Console.WriteLine();
                    //    highestPathCount = currentPath.PathCount;
                    //    Console.WriteLine($"Highest Path Count set to {highestPathCount}");
                    //}

                    if (_bestLength > currentPath.Length)
                    {
                        Console.WriteLine();
                        _bestLength = currentPath.Length;
                        Console.WriteLine($"Best Length set to {_bestLength}");
                    }

                    continue;
                }

                //var minPathLength = possibleDestinations.Min(d => d.Retrace.Path.Length);

                var destinations = possibleDestinations.OrderBy(d => d.Length).ToList();

                foreach (var possibleDestination in destinations)
                {
                    var newCollectedKeys = new List<char>(currentPath.CollectedKeys)
                        {
                            possibleDestination.To
                        };

                    var newLength = currentPath.Length + possibleDestination.Length;

                    paths.Add(new Path
                    {
                        Depth = currentPath.Depth + 1,
                        Length = newLength,
                        CollectedKeys = newCollectedKeys,
                        LastPosition = possibleDestination.To
                        //PathCount = currentPath.PathCount * destinations.Count
                    });
                }
            }

            return _bestLength;
        }

        //private static void DrawPath(Grid grid, Path drawPath)
        //{
        //    Console.BackgroundColor = ConsoleColor.DarkMagenta;
        //    Console.ForegroundColor = ConsoleColor.Yellow;

        //    for (int index = 0; index < drawPath.Length; index++)
        //    {
        //        var coordinate = drawPath.Retrace.Path[index];

        //        if (coordinate.X < 0 && coordinate.X > 81 && coordinate.Y < 0 && coordinate.Y > 80)
        //        {
        //            continue;
        //        }

        //        Console.SetCursorPosition(coordinate.X, coordinate.Y);
        //        Console.Write("·");
        //    }

        //    Console.BackgroundColor = ConsoleColor.Black;
        //    Console.ForegroundColor = ConsoleColor.White;
        //}

        private static void DrawGrid(Grid grid)
        {
            for (int index = 0; index < grid.Size.Width * grid.Size.Height; index++)
            {
                var y = index / grid.Size.Width;
                var x = index % grid.Size.Width;

                if (x >= 0 && x < 82 && y >= 0 && y < 80)
                {
                    Console.SetCursorPosition(x, y);
                    var character = grid.Map[index];

                    switch (character)
                    {
                        case '.': Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                        case '#': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                        case '@': Console.ForegroundColor = ConsoleColor.Green; break;
                        default:
                            if (character.ToString().ToUpper() == character.ToString())
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            break;
                    }

                    Console.Write(character);
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }



        public class Path
        {
            public int Depth { get; set; }
            // public Retrace Retrace { get; set; }
            // public Path Previous { get; set; }
            public char LastPosition { get; set; }
            public int Length { get; set; }
            public List<char> CollectedKeys { get; set; }
            //public long PathCount { get; set; }
        }
#endif



    }
}
