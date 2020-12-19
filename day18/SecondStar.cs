using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace day18
{
    public class SecondStar
    {
        private static Dictionary<char, Point> _keyPositions { get; set; }
        private static Dictionary<char, Point> _doorPositions { get; set; }
        private static List<Point> _robotPositions { get; set; }

        private static Dictionary<int, HashSet<char>> _keysPerQuadrant;

        private static Size _size;
        private static string[] _map;
        private static HashSet<string> _seen;

        private static Grid _grid;
        private static Pathfinder _pathfinder;

        private static Dictionary<char, int> _researched;

        public static string Run()
        {
            // 2144 correct
#if !MOCK
            _map = InputParser.Parse("input.txt");
#else
            _map = InputParser.Parse("mockInput.txt");
#endif
            _size = new Size(_map[0].Length, _map.Length);

            _keyPositions = new Dictionary<char, Point>();
            _doorPositions = new Dictionary<char, Point>();
            _robotPositions = new List<Point>();
            _seen = new HashSet<string>();
            _pathFindCache = new Dictionary<string, UncollectedKeyData>();

            _researched = new Dictionary<char, int>();

            // This is researched, so I have been looking at the maze and approximated
            // the priority of each key to help the Dynamic Programming be more on track.

            _researched.Add('r', 1);
            _researched.Add('a', 1);
            _researched.Add('g', 2);
            _researched.Add('o', 3);
            _researched.Add('i', 4);
            _researched.Add('c', 5);
            _researched.Add('y', 6);
            _researched.Add('m', 7);
            _researched.Add('h', 8);
            _researched.Add('u', 9);
            _researched.Add('d', 9);
            _researched.Add('p', 9);
            _researched.Add('n', 9);
            _researched.Add('k', 10);
            _researched.Add('x', 11);
            _researched.Add('t', 12);
            _researched.Add('e', 12);
            _researched.Add('j', 12);
            _researched.Add('q', 13);
            _researched.Add('b', 14);
            _researched.Add('w', 15);
            _researched.Add('z', 16);
            _researched.Add('v', 16);
            _researched.Add('s', 17);
            _researched.Add('l', 18);
            _researched.Add('f', 19);
#if !MOCK
            int editStartY = 39;
            var edit = "@#@###@#@";

            for (int y = 0; y < 3; y++)
                _map[editStartY + y] = _map[editStartY + y].Substring(0, 39) + edit.Substring(y * 3, 3) + _map[editStartY + y].Substring(42);
#endif

            _keysPerQuadrant = new Dictionary<int, HashSet<char>>();

            for (int y = 0; y < _size.Height; y++)
                for (int x = 0; x < _size.Width; x++)
                {
                    var point = new Point(x, y);
                    var character = _map[y][x];
                    if (character == '@')
                        _robotPositions.Add(point);
                    else if (character >= 'a' && character <= 'z')
                    {
                        _keyPositions.Add(character, point);
                        var quadrant = CalculateQuadrant(point);
                        if (!_keysPerQuadrant.ContainsKey(quadrant))
                            _keysPerQuadrant.Add(quadrant, new HashSet<char>());
                        _keysPerQuadrant[quadrant].Add(character);
                    }
                    else if (character >= 'A' && character <= 'Z')
                        _doorPositions.Add(character, point);
                }

            _grid = new Grid(_map, _size);
            _pathfinder = new Pathfinder();

            var robots = _robotPositions.Select(p => new Robot(p, 0)).ToArray();

            var startState = new State
            {
                Keys = new HashSet<char>(),
                Robots = robots
            };

            Explore(startState);

            return _shortestRoute.ToString(); // .Values.Sum(r => r.Distance).ToString();
        }

        private static int _shortestRoute = int.MaxValue;
        private static long _sinceLast = 0;
        private static long _cacheHits = 0;
        private static long _cacheMisses = 0;
        private static DateTime _since = DateTime.Now;

        private static void Explore(State state)
        {
            var hash = state.ToHash();

            if (_seen.Contains(hash))
                return;

            _seen.Add(hash);

            if (_seen.Count > _capacityMarkerSeen)
            {
                _capacityMarkerSeen *= 2;
                Console.WriteLine($"New capacity for SEEN cache: {_capacityMarkerSeen}");
            }

            if (state.Keys.Count == _keyPositions.Count) /* Have we won yet? */
            {
                if (state.Distance < _shortestRoute)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Score = {state.Distance}, Keys = {string.Join("", state.Keys)}");
                    _shortestRoute = state.Distance;
                }
                return;
            }

            // Pick Robot that goes first, based on nearest key (all robots considered)
            var uncollectedKeys = GetUncollectedUnblockedKeys(state);

            _sinceLast++;

            if (DateTime.Now > _since.AddSeconds(10))
            {
                long perSecond = _sinceLast / 10;
                long perSecondHits = _cacheHits / 10;
                long perSecondMisses = _cacheMisses / 10;

                Console.WriteLine($"Rates: {perSecond} checks/second, {perSecondHits} hits/second, {perSecondMisses} misses/second");

                _since = DateTime.Now;
                _sinceLast = _cacheHits = _cacheMisses = 0;
            }

            if (uncollectedKeys.Count == 0)
                return;

            foreach (var uncollectedKey in uncollectedKeys)
            {
                if (state.Keys.Contains(uncollectedKey.Key))
                    continue;

                var nextState = GoToKey(uncollectedKey, state);

                if (nextState.Distance > _shortestRoute)
                    continue;

                Explore(nextState);
            }
        }

        public struct UncollectedKeyData
        {
            public int RobotIndex;
            public char Key;
            public int Distance;
            public Point Position;
        }

        private static Dictionary<string, UncollectedKeyData> _pathFindCache;

        private static List<UncollectedKeyData> GetUncollectedUnblockedKeys(State state)
        {
            var uncollectedKeys = new List<UncollectedKeyData>();

            var quadrantKeys = new Dictionary<int, HashSet<char>>();

            for (int quadrant = 0; quadrant < 4; quadrant++)
            {
                var quadrantRobot = state.Robots.Where(r => CalculateQuadrant(r.Position) == quadrant).Single();
                var keys = _keysPerQuadrant[quadrant]
                    .Select(key => (key, _researched[key]))
                    .OrderBy(k => k.Item2)
                    .Select(k => k.key);

                quadrantKeys.Add(quadrant, new HashSet<char>());

                foreach (var key in keys)
                    quadrantKeys[quadrant].Add(key);
            }

            var collectedQuadrantKeys = new Dictionary<int, HashSet<char>>();
            for (int quadrant = 0; quadrant < 4; quadrant++)
            {
                collectedQuadrantKeys.Add(quadrant, new HashSet<char>());

                foreach (var key in state.Keys)
                    if (CalculateQuadrant(_keyPositions[key]) == quadrant)
                        collectedQuadrantKeys[quadrant].Add(key);
            }

            var nextKeyInResearchedPriority = _keyPositions.Keys
                .Where(key => !state.Keys.Contains(key))
                .Select(key => (key, _researched[key]))
                .OrderBy(k => k.Item2)
                .Select(k => k.key)
                .First();

            var allKeysThatSharePriority = _researched
                .Where(r => r.Value == _researched[nextKeyInResearchedPriority] && !state.Keys.Contains(r.Key))
                .Select(p => p.Key);

            foreach (var priorityKey in allKeysThatSharePriority)
            {
                if (state.Keys.Contains(priorityKey))
                    throw new Exception("Should not happen.");

                var quadrant = CalculateQuadrant(_keyPositions[priorityKey]);
                var robot = state.Robots[quadrant];

                var keys = string.Join("", collectedQuadrantKeys[quadrant]);
                var hash = $"{robot.Position.X}/{robot.Position.Y},{keys},{priorityKey}";

                if (_pathFindCache.ContainsKey(hash))
                {
                    _cacheHits++;
                    uncollectedKeys.Add(_pathFindCache[hash]);

                    continue;
                }
                _cacheMisses++;

                var retrace = _pathfinder.FindPath(robot.Position, _keyPositions[priorityKey], _grid, state.Keys);

                if (retrace != null)
                {
                    var uncollected = new UncollectedKeyData
                    {
                        Key = priorityKey,
                        Distance = retrace.Path.Length,
                        Position = _keyPositions[priorityKey],
                        RobotIndex = quadrant
                    };

                    uncollectedKeys.Add(uncollected);

                    _pathFindCache.Add(hash, uncollected);

                    if (_pathFindCache.Count > _capacityMarkerPathFind)
                    {
                        _capacityMarkerPathFind *= 2;
                        Console.WriteLine($"New capacity for PATH FIND cache: {_capacityMarkerPathFind}");
                    }
                }
            }

            return uncollectedKeys;
        }

        private static long _capacityMarkerPathFind = 2048;
        private static long _capacityMarkerSeen = 4096;

        private static State GoToKey(UncollectedKeyData uncollectedKey, State state)
        {
            var nextState = new State
            {
                Robots = new Robot[]
                {
                    new Robot
                    {
                        Position = state.Robots[0].Position,
                        Distance = state.Robots[0].Distance
                    },
                    new Robot
                    {
                        Position = state.Robots[1].Position,
                        Distance = state.Robots[1].Distance
                    },
                    new Robot
                    {
                        Position = state.Robots[2].Position,
                        Distance = state.Robots[2].Distance
                    },
                    new Robot
                    {
                        Position = state.Robots[3].Position,
                        Distance = state.Robots[3].Distance
                    }
                },
                Keys = new HashSet<char>(state.Keys)
            };

            nextState.Keys.Add(uncollectedKey.Key);
            nextState.Robots[uncollectedKey.RobotIndex].Position = uncollectedKey.Position;
            nextState.Robots[uncollectedKey.RobotIndex].Distance += uncollectedKey.Distance;
            nextState.Distance = state.Distance + uncollectedKey.Distance;

            return nextState;
        }

        private static int CalculateQuadrant(Point location)
            => (location.X < _size.Width / 2 ? 0 : 1) + (location.Y < _size.Height / 2 ? 0 : 2);
    }
}
