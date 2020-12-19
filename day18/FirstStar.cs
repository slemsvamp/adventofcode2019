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

        private static Size _size;
        private static string[] _map;
        private static HashSet<string> _seen;

        private static int[] _dirX = new[] { -1, 0, 1, 0 };
        private static int[] _dirY = new[] { 0, -1, 0, 1 };

        private const char WALL = '#';

        public static string Run()
        {
            //_map = InputParser.Parse("input.txt");
            //_size = new Size(_map[0].Length, _map.Length);

            //_keyPositions = new Dictionary<char, Point>();
            //_doorPositions = new Dictionary<char, Point>();

            //for (int y = 0; y < _size.Height; y++)
            //    for (int x = 0; x < _size.Width; x++)
            //    {
            //        var point = new Point(x, y);
            //        var character = _map[y][x];
            //        if (character == '@')
            //            _characterPosition = point;
            //        else if (character >= 'a' && character <= 'z')
            //            _keyPositions.Add(character, point);
            //        else if (character >= 'A' && character <= 'Z')
            //            _doorPositions.Add(character, point);
            //    }

            //_seen = new HashSet<string>();

            //var queue = new Queue<State>();
            //queue.Enqueue(new State
            //{
            //    X = _characterPosition.X,
            //    Y = _characterPosition.Y,
            //    Keys = new HashSet<char>(),
            //    Depth = 0
            //});

            //while (queue.Count > 0)
            //{
            //    var state = queue.Dequeue();

            //    var sortedKeys = string.Empty;
            //    foreach (var k in state.Keys.OrderBy(k => k))
            //        sortedKeys += k;

            //    var index = state.Y * _size.Width + state.X;

            //    string key = $"{index}{sortedKeys}";

            //    if (_seen.Contains(key))
            //        continue;
            //    _seen.Add(key);

            //    if (_size.Width <= state.X && state.X < 0 && _size.Height <= state.Y && state.Y < 0)
            //        continue;

            //    var c = _map[state.Y][state.X];

            //    if (c == WALL)
            //        continue;

            //    if (_doorPositions.ContainsKey(c) && state.Keys.Contains(c.ToString().ToLower()[0]) == false)
            //        continue;

            //    var newKeys = new HashSet<char>(state.Keys);

            //    if (_keyPositions.ContainsKey(c))
            //    {
            //        if (!newKeys.Contains(c))
            //            newKeys.Add(c);
            //        if (newKeys.Count == _keyPositions.Count)
            //            return state.Depth.ToString();
            //    }

            //    for (int direction = 0; direction < 4; direction++)
            //        queue.Enqueue(new State
            //        {
            //            X = state.X + _dirX[direction],
            //            Y = state.Y + _dirY[direction],
            //            Keys = newKeys,
            //            Depth = state.Depth + 1
            //        });
            //}

            return "FAILED";
        }
    }
}
