using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day13
{
    public class FirstStar
    {
        public enum TileType
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HorizontalPaddle = 3,
            Ball = 4
        }

        public struct Coordinate
        {
            public long X;
            public long Y;

            public Coordinate(long x, long y)
            {
                X = x;
                Y = y;
            }
        }

        public class Tile
        {
            public Coordinate Position { get; set; }
            public TileType Type { get; set; }
        }

        public static string Run(List<long> opcodes)
        {
            var opcodeRunner = new OpcodeRunner(5000, opcodes, null);

            var tileList = new List<Tile>();

            var tl = new Coordinate(0, 0);
            var br = new Coordinate(0, 0);

            int blockTileCount = 0;

            while (!opcodeRunner.Halted)
            {
                long left = opcodeRunner.Run();
                long top = opcodeRunner.Run();
                TileType type = (TileType)opcodeRunner.Run();

                tileList.Add(new Tile
                {
                    Position = new Coordinate(left, top),
                    Type = type
                });

                if (tl.X > left) tl.X = left;
                if (tl.Y > top) tl.Y = top;
                if (br.X < left) br.X = left;
                if (br.Y < top) br.Y = top;

                if (type == TileType.Block)
                {
                    blockTileCount++;
                }
            }

            var screenSize = new Size((int)(br.X - tl.X + 1), (int)(br.Y - tl.Y + 1));
            var tiles = new Tile[screenSize.Width * screenSize.Height];

            for (int tileIndex = 0; tileIndex < tileList.Count; tileIndex++)
            {
                tiles[tileIndex] = tileList[tileIndex];
            }

            return blockTileCount.ToString();
        }
    }
}
