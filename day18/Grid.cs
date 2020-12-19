using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace day18
{
    public class Grid : IGrid
    {
        private readonly Size _size;

        public Size Size => _size;
        
        public char[] Map { get; set; }

        public Grid(string[] map, Size size)
        {
            _size = size;
            
            Map = new char[size.Width * size.Height];

            for (int y = 0; y < size.Height; y++)
                for (int x = 0; x < size.Width; x++)
                    Map[x + y * size.Width] = map[y][x];
        }

        //public List<RetraceData> GenerateRetraces(Dictionary<char, Point> keyPositions, Dictionary<char, Point> doorPositions, Point characterPosition)
        //{
        //    var pathfinder = new Pathfinder();
        //    var retraces = new List<RetraceData>();

        //    var allPositions = new Dictionary<char, Point>();
        //    foreach (var keyValuePair in keyPositions)
        //        allPositions.Add(keyValuePair.Key, keyValuePair.Value);
        //    foreach (var keyValuePair in doorPositions)
        //        allPositions.Add(keyValuePair.Key, keyValuePair.Value);

        //    foreach (var endKeyValuePair in allPositions)
        //    {
        //        var endKey = endKeyValuePair.Key;
        //        var endPosition = endKeyValuePair.Value;

        //        var retracePath = pathfinder.FindPath(characterPosition, endPosition, this, );

        //        retraces.Add(new RetraceData
        //        {
        //            From = '@',
        //            To = endKey,
        //            Doors = retracePath.Doors,
        //            Length = retracePath.Path.Length
        //        });
        //    }

        //    foreach (var startKeyValuePair in allPositions)
        //    {
        //        var startKey = startKeyValuePair.Key;
        //        var startPosition = startKeyValuePair.Value;

        //        foreach (var endKeyValuePair in allPositions)
        //        {
        //            var endKey = endKeyValuePair.Key;
        //            var endPosition = endKeyValuePair.Value;

        //            var pair = retraces.Where(r => r.From == endKey && r.To == startKey).SingleOrDefault();

        //            if (startKey == endKey)
        //            {
        //                continue;
        //            }
        //            else if (pair != null)
        //            {
        //                retraces.Add(new RetraceData
        //                {
        //                    From = startKey,
        //                    To = endKey,
        //                    Doors = pair.Doors,
        //                    Length = pair.Length
        //                });

        //                continue;
        //            }

        //            var retracePath = pathfinder.FindPath(startPosition, endPosition, this);

        //            retraces.Add(new RetraceData
        //            {
        //                From = startKey,
        //                To = endKey,
        //                Doors = retracePath.Doors,
        //                Length = retracePath.Path.Length
        //            });
        //        }
        //    }

        //    return retraces;
        //}

        public void SetPoint(Point point, char data)
        {
            Map[point.Y * _size.Width + point.X] = data;
        }

        public int GetMaximumHeapSize()
        {
            return _size.Width * _size.Height;
        }

        public PathfinderNode[] GetNeighbours(Point point, HashSet<char> keys)
        {
            if (point.X > 0 && point.X < _size.Width && point.Y > 0 && point.Y < _size.Height)
            {
                var isDoor = new Func<char, bool>(c => c >= 'A' && c <= 'Z');
                Point up = point.Add(0, -1), down = point.Add(0, 1), left = point.Add(-1, 0), right = point.Add(1, 0);

                var nodes = new List<PathfinderNode>();

                foreach (var location in new Point[] { up, down, left, right })
                    nodes.Add(GetNodeFromWorldPoint(location, keys));

                return nodes.ToArray();
            }

            return new PathfinderNode[4]
            {
                new PathfinderNode(point.Add(0, -1), int.MaxValue),
                new PathfinderNode(point.Add(0, 1), int.MaxValue),
                new PathfinderNode(point.Add(-1, 0), int.MaxValue),
                new PathfinderNode(point.Add(1, 0), int.MaxValue)
            };
        }

        private bool IsDoor(char terrain)
            => terrain >= 'A' && terrain <= 'Z';

        private bool IsKey(char terrain)
            => terrain >= 'a' && terrain <= 'z';

        public PathfinderNode GetNodeFromWorldPoint(Point point, HashSet<char> keys)
        {
            var terrain = Map[point.Y * _size.Width + point.X];
            var cost = int.MaxValue; /* wall or locked door is default */
            if (terrain == '.' /* open space */ || terrain == '@' /* character start */ ||
                (IsDoor(terrain) && keys.Contains(terrain.ToString().ToLower()[0])) /* passable door */ ||
                IsKey(terrain))
            {
                cost = 1;
            }

            return new PathfinderNode(point, cost);
        }

        public void Draw()
        {
            for (int index = 0; index < _size.Width * _size.Height; index++)
            {
                var y = index / _size.Width;
                var x = index % _size.Width;

                if (x >= 0 && x < Program.ScreenWidth && y >= 0 && y < Program.ScreenHeight)
                {
                    Console.SetCursorPosition(x, y);
                    var character = Map[index];

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
    }
}
