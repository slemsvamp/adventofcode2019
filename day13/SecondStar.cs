using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day13
{
    public class SecondStar
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

        public class GameState
        {
            public TileType[] Screen { get; set; }
            public int Width { get; set; }
            public long Score { get; set; }
            public int BlockCount { get; set; }
            public Coordinate Ball { get; set; }
            public Coordinate Paddle { get; set; }
            public Point BallMovement { get; set; }
        }

        public class Backup
        {
            public GameState State { get; set; }
            public long[] Opcodes { get; set; }
            public int Playhead { get; set; }
            public Queue<long> Input { get; set; }
        }

        public static string Run(List<long> opcodes)
        {
            // 10710 is too low

            opcodes[0] = 2;

            var opcodeRunner = new OpcodeRunner(5000, opcodes);

            // send 0, get the entire field

            // listen for an empty?
            // listen for a ball type
            // input

            GameState state = GetGameState(opcodeRunner);

            do
            {
                bool failed = false;

                state = RunOnce(state, opcodeRunner);

                if (state.Ball.Y > state.Paddle.Y)
                {
                    return "GAME OVER";
                }

                DrawScreen(state);
                Console.ReadKey(true);

                if (!failed)
                {
                    opcodeRunner.Halted = false;
                }
            } while (state.BlockCount > 0);

            return state.Score.ToString();
        }

        private static void DrawScreen(GameState state)
        {
            Console.Clear();

            for (int tileIndex = 0; tileIndex < state.Screen.Length; tileIndex++)
            {
                Console.SetCursorPosition(tileIndex % state.Width, tileIndex / state.Width);
                switch (state.Screen[tileIndex])
                {
                    case TileType.Ball: Console.Write("o"); break;
                    case TileType.Wall: Console.Write("░"); break;
                    case TileType.Block: Console.Write("#"); break;
                    case TileType.HorizontalPaddle: Console.Write("="); break;
                    default:
                    break;
                }
            }
            Console.SetCursorPosition(1, 0);
            Console.WriteLine(" Score: " + state.Score + " ");

            Console.SetCursorPosition(2, 22);
            Console.WriteLine("Blocks: " + state.BlockCount);
        }

        public static GameState GetGameState(OpcodeRunner opcodeRunner)
        {
            // run it until score
            var tl = new Coordinate(0, 0);
            var br = new Coordinate(0, 0);
            var tileList = new List<Tile>();
            var paddle = new Coordinate(0, 0);
            var ball = new Coordinate(0, 0);
            var blockCount = 0;

            opcodeRunner.AddInput(0);

            while (true)
            {
                long left = opcodeRunner.Run();
                long top = opcodeRunner.Run();

                if (left == -1)
                {
                    long score = opcodeRunner.Run();

                    var screenSize = new Size((int)(br.X - tl.X + 1), (int)(br.Y - tl.Y + 1));
                    var tiles = new TileType[screenSize.Width * screenSize.Height];

                    foreach (var tile in tileList)
                    {
                        var tileIndex = tile.Position.Y * screenSize.Width + tile.Position.X;
                        tiles[tileIndex] = tile.Type;
                    }

                    return new GameState
                    {
                        Screen = tiles,
                        BlockCount = blockCount,
                        Score = score,
                        Width = screenSize.Width,
                        Ball = ball,
                        Paddle = paddle,
                        BallMovement = new Point(1, 1)
                    };
                }
                else
                {
                    var type = (TileType)opcodeRunner.Run();

                    switch (type)
                    {
                        case TileType.Ball:
                        {
                            ball = new Coordinate(left, top);
                        }
                        break;
                        case TileType.HorizontalPaddle:
                        {
                            paddle = new Coordinate(left, top);
                        }
                        break;
                        case TileType.Block:
                        {
                            blockCount++;
                        }
                        break;
                        default:
                        break;
                    }

                    tileList.Add(new Tile
                    {
                        Position = new Coordinate(left, top),
                        Type = type
                    });
                }

                if (tl.X > left) tl.X = left;
                if (tl.Y > top) tl.Y = top;
                if (br.X < left) br.X = left;
                if (br.Y < top) br.Y = top;
            }
        }

        public static GameState RunOnce(GameState state, OpcodeRunner opcodeRunner)
        {
            var joystick = CalculateJoystick(state);

            var left = opcodeRunner.Run();
            var top = opcodeRunner.Run();
            
            if (left == -1)
            {
                var score = opcodeRunner.Run();
            }
            else
            {
                var type = (TileType)opcodeRunner.Run();

                state.Screen[top * state.Width + left] = type;

                switch (type)
                {
                    case TileType.Ball:
                    {
                        state.Ball = new Coordinate(left, top);
                    }
                    break;
                }
            }

            return state;
        }

        private static long CalculateJoystick(GameState state)
        {
            long ballX = state.Ball.X;
            long ballY = state.Ball.Y;

            if (state.BallMovement.Y == 1)
            {
                if (state.BallMovement.X == 1)
                {
                    while (ballY < state.Paddle.Y - 1)
                    {
                        ballX++;
                        ballY++;
                    }
                }
                else
                {
                    while (ballY < state.Paddle.Y - 1)
                    {
                        ballX--;
                        ballY++;
                    }
                }
            }
            else
            {
                if (state.BallMovement.X < 0)
                {
                    ballX--;
                }
                else
                {
                    ballX++;
                }
            }

            if (ballX < state.Paddle.X)
            {
                return -1;
            }
            else if (ballX > state.Paddle.X)
            {
                return 1;
            }

            return 0;
        }
    }
}
