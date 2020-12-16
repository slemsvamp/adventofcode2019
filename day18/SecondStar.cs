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
        public class Character
        {
            public Queue<State> Queue;
            public bool Done;
            public int Depth;
            public int Quadrant;
            public HashSet<char> Stall;

            public Character()
            {
                Queue = new Queue<State>();
                Done = false;
                Depth = 0;
                Quadrant = -1;
                Stall = new HashSet<char>();
            }
        }

        public class Robot
        {
            public int Quadrant;
            public Point Position;
            public int Distance;

            public Robot(int quadrant, Point position)
            {
                Quadrant = quadrant;
                Position = position;
                Distance = 0;
            }
        }

        private static Dictionary<char, Point> _keyPositions { get; set; }
        private static Dictionary<char, Point> _doorPositions { get; set; }
        private static List<Point> _characterPositions { get; set; }

        private static Dictionary<int, HashSet<char>> _keysPerQuadrant;

        private static Size _size;
        private static string[] _map;
        private static HashSet<string> _seen;
        private static HashSet<char> _sharedKeys;
        private static Dictionary<int, Robot> _robots;

        private static int[] _dirX = new[] { -1, 0, 1, 0 };
        private static int[] _dirY = new[] { 0, -1, 0, 1 };

        private const char WALL = '#';

        public static string RunOld()
        {
            // 1408, too low
            // 2996, too high

            _map = InputParser.Parse("mockInput.txt");
            _size = new Size(_map[0].Length, _map.Length);

            _keyPositions = new Dictionary<char, Point>();
            _doorPositions = new Dictionary<char, Point>();
            _characterPositions = new List<Point>();

            //int editStartY = 39;
            //var edit = "@#@###@#@";

            //for (int y = 0; y < 3; y++)
            //    _map[editStartY + y] = _map[editStartY + y].Substring(0, 39) + edit.Substring(y * 3, 3) + _map[editStartY + y].Substring(42);

            int drawAt = 0; // 40;

            for (int y = drawAt; y < drawAt + 9 /*40*/; y++)
                Console.WriteLine(_map[y]);

            _keysPerQuadrant = new Dictionary<int, HashSet<char>>();

            for (int y = 0; y < _size.Height; y++)
                for (int x = 0; x < _size.Width; x++)
                {
                    var point = new Point(x, y);
                    var character = _map[y][x];
                    if (character == '@')
                        _characterPositions.Add(point);
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

            _seen = new HashSet<string>();

            var characters = new List<Character>();
            foreach (var position in _characterPositions)
            {
                var character = new Character
                {
                    Quadrant = CalculateQuadrant(position)
                };

                character.Queue.Enqueue(new State()
                {
                    X = position.X,
                    Y = position.Y,
                    Keys = new HashSet<char>(),
                    Depth = 0
                });
                characters.Add(character);
            }

            int complete = 0;
            var sharedKeys = new HashSet<char>();

            while (complete < 4)
            {
                foreach (var character in characters)
                {
                    var debug = false;

                    //if (character.Quadrant == 3)
                    //    debug = true;

                    if (character.Done)
                        continue;

                    if (character.Queue.Count == 0)
                    {
                        complete++;
                        character.Done = true;
                    }

                    // Skip character because stalled, need key..!
                    if (character.Stall.Count > 0)
                    {
                        bool tryAgain = false;
                        var checkSet = new HashSet<char>(character.Stall);
                        foreach (var checkChar in checkSet)
                            if (sharedKeys.Contains(checkChar))
                            {
                                character.Stall.Remove(checkChar);
                                tryAgain = true;
                            }

                        if (!tryAgain)
                            continue;
                    }

                    var state = character.Queue.Dequeue();

                    if (debug) Debug.WriteLine($"Character {character.Quadrant} dequeued, {character.Queue.Count} state(s) left");

                    var sortedKeys = string.Empty;
                    foreach (var k in sharedKeys)
                        sortedKeys += k;

                    var index = state.Y * _size.Width + state.X;

                    string key = $"{index}{sortedKeys}";

                    if (_size.Width <= state.X && state.X < 0 && _size.Height <= state.Y && state.Y < 0)
                    {
                        if (debug) Debug.WriteLine($"Character {character.Quadrant} met up with a map limit");
                        continue;
                    }

                    var c = _map[state.Y][state.X];

                    if (_seen.Contains(key) && c < 'A' && c > 'Z')
                    {
                        if (debug) Debug.WriteLine($"Character {character.Quadrant} has seen {state.X}x/{state.Y}y, sortedKeys={sortedKeys}");
                        continue;
                    }
                    _seen.Add(key);


                    if (state.Y >= drawAt && state.Y - drawAt < 50)
                        Console.SetCursorPosition(state.X, state.Y - drawAt);

                    //Thread.Sleep(5);

                    if (c == WALL)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("#");

                        if (debug) Debug.WriteLine($"Character {character.Quadrant} met up with a wall at {state.X}x/{state.Y}y");
                        continue;
                    }

                    if (_doorPositions.ContainsKey(c) && sharedKeys.Contains(c.ToString().ToLower()[0]) == false)
                    {
                        var newQueue = new Queue<State>();
                        newQueue.Enqueue(new State
                        {
                            X = state.X,
                            Y = state.Y,
                            Depth = state.Depth
                        });
                        foreach (var stateInCharacterQueue in character.Queue)
                            newQueue.Enqueue(stateInCharacterQueue);

                        character.Queue = newQueue;

                        character.Stall.Add(c.ToString().ToLower()[0]);

                        if (debug) Debug.WriteLine($"Character {character.Quadrant} can't open door '{c}', stalling. queueLength={character.Queue.Count}");
                        continue;
                    }

                    if (_keyPositions.ContainsKey(c))
                    {
                        sharedKeys.Add(c);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(c);

                        if (debug) Debug.WriteLine($"Character {character.Quadrant} picked up key '{c}'");

                        var keysInQuadrant = _keysPerQuadrant[CalculateQuadrant(new Point(state.X, state.Y))];

                        if (keysInQuadrant.All(key => sharedKeys.Contains(key)))
                        {
                            if (debug) Debug.WriteLine($"Character {character.Quadrant} have all the keys in quadrant {character.Quadrant}, keysInQuadrant={string.Join(",", keysInQuadrant)}");
                            character.Depth = state.Depth;
                            character.Done = true;
                            complete++;
                            continue;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(".");
                    }

                    for (int direction = 0; direction < 4; direction++)
                        character.Queue.Enqueue(new State
                        {
                            X = state.X + _dirX[direction],
                            Y = state.Y + _dirY[direction],
                            Depth = state.Depth + 1
                        });
                }
            }

            //Console.ForegroundColor = ConsoleColor.White;
            //foreach (var character in characters)
            //    Console.WriteLine(character.Depth);

            Console.ReadKey(true);

            return characters.Sum(c => c.Depth).ToString();
        }

        public static string Run()
        {
#if MOCK
            _map = InputParser.Parse("input.txt");
#else
            _map = InputParser.Parse("mockInput.txt");
#endif
            _size = new Size(_map[0].Length, _map.Length);

            _keyPositions = new Dictionary<char, Point>();
            _doorPositions = new Dictionary<char, Point>();
            _characterPositions = new List<Point>();
            _robots = new Dictionary<int, Robot>();

#if MOCK
            int editStartY = 39;
            var edit = "@#@###@#@";

            for (int y = 0; y < 3; y++)
                _map[editStartY + y] = _map[editStartY + y].Substring(0, 39) + edit.Substring(y * 3, 3) + _map[editStartY + y].Substring(42);
#endif

            _sharedKeys = new HashSet<char>();
            _keysPerQuadrant = new Dictionary<int, HashSet<char>>();

            for (int y = 0; y < _size.Height; y++)
                for (int x = 0; x < _size.Width; x++)
                {
                    var point = new Point(x, y);
                    var character = _map[y][x];
                    if (character == '@')
                        _characterPositions.Add(point);
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

            var grid = new Grid(_map, _size);
            var pathfinder = new Pathfinder();

            _robots = new Dictionary<int, Robot>();
            foreach (var position in _characterPositions)
            {
                var quadrant = CalculateQuadrant(position);
                var robot = new Robot(quadrant, position);
                _robots.Add(quadrant, robot);
            }

            // Pick Robot that goes first, based on nearest key (all robots considered)
            var activeRobot = GetStartingRobot(pathfinder, grid);

            while (true)
            {
                if (_sharedKeys.Count == _keyPositions.Count) /* Have we won yet? */
                    break;

                // Go get the key, shortest path and record state (steps/depth)
                var key = GoToNearestUncollectedUnblockedKey(activeRobot, pathfinder, grid);

                Draw(activeRobot, grid);

                Console.SetCursorPosition(0, grid.Size.Height + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Robot {activeRobot.Quadrant} picked up key '{key}'.");
                
                Console.ReadKey(true);
                
                // Calculate and set next Robot to act next, based on nearest key (all robots considered)
                activeRobot = ChooseNextRobot(pathfinder, grid);
            }

            return _robots.Values.Sum(r => r.Distance).ToString();
        }

        private static void Draw(Robot robot, Grid grid)
        {
            int minY = robot.Position.Y - 20;
            int maxY = robot.Position.Y + 20;

            if (minY < 0)
                (minY, maxY) = (0, 40);
            //if (maxY >= grid.Size.Height)
            //    (minY, maxY) = (grid.Size.Height, grid.Size.Height - 1);

            minY = 0;
            maxY = grid.Size.Height;

            Console.Clear();
            for (int y = minY; y < maxY; y++)
            {
                for (int x = 0; x < grid.Size.Width; x++)
                {
                    Console.SetCursorPosition(x, y - minY);
                    var terrain = grid.Map[y * grid.Size.Width + x];

                    switch (terrain)
                    {
                        case '.': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                        case '@': Console.ForegroundColor = ConsoleColor.Green; break;
                        case '#': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                        default:
                            if (terrain >= 'a' && terrain <= 'z')
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            else if (terrain >= 'A' && terrain <= 'Z')
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                    }

                    Console.Write(terrain);
                }
            }

            Console.SetCursorPosition(robot.Position.X, robot.Position.Y - minY);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("☻");
        }

        private static Robot ChooseNextRobot(Pathfinder pathfinder, IGrid grid)
        {
            var shortestDistance = int.MaxValue;
            var shortestDistanceRobot = (Robot)null;

            foreach (var robot in _robots.Values)
            {
                foreach (var key in _keysPerQuadrant[robot.Quadrant])
                {
                    if (_sharedKeys.Contains(key))
                        continue;

                    var retrace = pathfinder.FindPath(robot.Position, _keyPositions[key], grid, _sharedKeys);

                    if (retrace != null)
                    {
                        var length = retrace.Path.Length;

                        if (length < shortestDistance)
                        {
                            length = shortestDistance;
                            shortestDistanceRobot = robot;
                        }
                    }
                }
            }

            return shortestDistanceRobot;
        }

        private static char GoToNearestUncollectedUnblockedKey(Robot robot, Pathfinder pathfinder, Grid grid)
        {
            var shortestDistance = int.MaxValue;
            var shortestDistanceEndPosition = Point.Empty;
            var shortestDistanceKey = '\0';
            foreach (var key in _keysPerQuadrant[robot.Quadrant])
            {
                if (_sharedKeys.Contains(key))
                    continue;

                var keyPosition = _keyPositions[key];
                var retrace = pathfinder.FindPath(robot.Position, keyPosition, grid, _sharedKeys);

                if (retrace != null)
                {
                    if (retrace.Path.Length < shortestDistance)
                    {
                        shortestDistanceEndPosition = retrace.Path.Last();
                        shortestDistance = retrace.Path.Length;
                        shortestDistanceKey = key;
                    }
                }

            }

            if (shortestDistance < int.MaxValue)
            {
                robot.Position = shortestDistanceEndPosition;
                robot.Distance += shortestDistance;
                _sharedKeys.Add(shortestDistanceKey);
            }

            return shortestDistanceKey;
        }

        private static Robot GetStartingRobot(Pathfinder pathfinder, IGrid grid)
        {
            var shortestLength = int.MaxValue;
            var shortestLengthRobot = (Robot)null;

            foreach (var keyValuePair in _keyPositions)
            {
                var quadrant = CalculateQuadrant(keyValuePair.Value);
                var robot = _robots[quadrant];
                var retrace = pathfinder.FindPath(robot.Position, keyValuePair.Value, grid, new HashSet<char>());

                if (retrace != null)
                { 
                    if (retrace.Path.Length < shortestLength)
                    {
                        shortestLength = retrace.Path.Length;
                        shortestLengthRobot = robot;
                    }
                }
            }

            return shortestLengthRobot;
        }

        private static int CalculateQuadrant(Point location)
            => (location.X < _size.Width / 2 ? 0 : 1) +
                (location.Y < _size.Height / 2 ? 0 : 2);
    }
}
