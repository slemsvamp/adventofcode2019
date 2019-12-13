using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

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

        private static GameState State { get; set; }

        public static string Run(List<long> opcodes, bool draw)
        {
            // 10710 is too low
            // 17384 is too low
            // 3073163 is too high

            opcodes[0] = 2;

            var opcodeRunner = new OpcodeRunner(5000, opcodes, CalculateJoystick);

            // send 0, get the entire field

            // listen for an empty?
            // listen for a ball type
            // input

            State = GetGameState(opcodeRunner);
            if (draw)
            {
                DrawScreen();
            }

            do
            {
                bool failed = false;

                RunOnce(opcodeRunner, draw);

                if (!failed)
                {
                    opcodeRunner.Halted = false;
                }
            } while (State.BlockCount > 0);

            // Updates the Score
            RunOnce(opcodeRunner, draw);

            if (!draw)
            {
                Console.Clear();
                Console.WriteLine("YOU WIN! FINAL SCORE: " + State.Score.ToString());
            }

            return "";
        }

        private static void DrawScreen()
        {
            for (int tileIndex = 0; tileIndex < State.Screen.Length; tileIndex++)
            {
                Console.SetCursorPosition(tileIndex % State.Width, tileIndex / State.Width);
                switch (State.Screen[tileIndex])
                {
                    case TileType.Ball: Console.Write("o"); break;
                    case TileType.Wall: Console.Write("░"); break;
                    case TileType.Block: Console.Write("#"); break;
                    case TileType.Empty: Console.Write(" "); break;
                    case TileType.HorizontalPaddle: Console.Write("="); break;
                    default:
                    break;
                }
            }

            Console.SetCursorPosition(1, 0);
            Console.WriteLine(" Score: " + State.Score + " ");

            Console.SetCursorPosition(2, 22);
            Console.WriteLine("Blocks: " + State.BlockCount + "  ");
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

        public static void RunOnce(OpcodeRunner opcodeRunner, bool draw)
        {
            var left = opcodeRunner.Run();
            var top = opcodeRunner.Run();
            
            if (left == -1)
            {
                State.Score = opcodeRunner.Run();
            }
            else
            {
                var type = (TileType)opcodeRunner.Run();

                if (State.Screen[top * State.Width + left] == TileType.Block)
                {
                    State.BlockCount--;
                }

                if (draw)
                {
                    Console.SetCursorPosition((int)left, (int)top);
                }

                switch (type)
                {
                    case TileType.Ball:
                    {
                        if (State.Ball.Y < top)
                        {
                            State.BallMovement = new Point(State.BallMovement.X, 1);
                        }
                        else if (State.Ball.Y > top)
                        {
                            State.BallMovement = new Point(State.BallMovement.X, -1);
                        }

                        if (State.Ball.X > left)
                        {
                            State.BallMovement = new Point(-1, State.BallMovement.Y);
                        }
                        else if (State.Ball.X < left)
                        {
                            State.BallMovement = new Point(1, State.BallMovement.Y);
                        }

                        State.Ball = new Coordinate(left, top);

                        if (draw)
                        {
                            Console.Write("o");
                            Thread.Sleep(30);
                        }
                    }
                    break;
                    case TileType.HorizontalPaddle:
                    {
                        State.Paddle = new Coordinate(left, top);

                        if (draw)
                        {
                            Console.Write("=");
                            Thread.Sleep(30);
                        }
                    }
                    break;
                    case TileType.Empty:
                    {
                        if (draw)
                        {
                            Console.Write(" ");
                        }
                    }
                    break;
                }

                State.Screen[top * State.Width + left] = type;
            }

            if (draw)
            {
                Console.SetCursorPosition(1, 0);
                Console.WriteLine(" Score: " + State.Score + " ");

                Console.SetCursorPosition(2, 22);
                Console.WriteLine("Blocks: " + State.BlockCount + "  ");
            }
        }

        private static long CalculateJoystick()
        {
            long ballX = State.Ball.X;
            long ballY = State.Ball.Y;

            if (State.BallMovement.Y == 1)
            {
                if (State.BallMovement.X == 1)
                {
                    while (ballY < State.Paddle.Y - 1)
                    {
                        ballX++;
                        ballY++;
                    }
                }
                else
                {
                    while (ballY < State.Paddle.Y - 1)
                    {
                        ballX--;
                        ballY++;
                    }
                }
            }
            else
            {
                if (State.BallMovement.X < 0)
                {
                    ballX--;
                }
                else
                {
                    ballX++;
                }
            }

            if (ballX < State.Paddle.X)
            {
                return -1;
            }
            else if (ballX > State.Paddle.X)
            {
                return 1;
            }

            return 0;
        }
    }
}
