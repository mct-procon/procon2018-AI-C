using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Boards
{
    public class BoardSetting
    {
        public Player Me { get; private set; }
        public Player Enemy { get; private set; }

        public sbyte[,] ScoreBoard { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private BoardSetting() { }

        public static BoardSetting Generate(byte height = 20, byte width = 20)
        {
            if ((width & 0b1) == 1)
                throw new ArgumentException("width must be an odd number.", nameof(width));


            BoardSetting result = new BoardSetting();
            result.Width = width;
            result.Height = height;
            result.ScoreBoard = new sbyte[height, width];

            Random rand = new Random();

            int widthDiv2 = width / 2;

            for(int y  = 0; y < height; ++y)
                for(int x = 0; x < widthDiv2; ++x)
                {
                    int value = rand.Next(16 * 3);
                    value -= 16;
                    if (value > 0)
                        value /= 2;
                    sbyte value_s = (sbyte)value;
                    result.ScoreBoard[y, x] = value_s;
                    result.ScoreBoard[y, result.ScoreBoard.GetLength(1) - x] = value_s;
                }

            int heightDiv2 = height / 2;

            ushort agent1_y = (ushort)rand.Next(heightDiv2);
            ushort agent2_y = (ushort)(rand.Next(heightDiv2) + heightDiv2);

            int agent1_x = rand.Next(widthDiv2);
            int agent2_x = rand.Next(widthDiv2);

            result.Me = new Player(new Point((ushort)agent1_x, agent1_y), new Point((ushort)agent2_x, agent2_y));
            result.Enemy = new Player(new Point((ushort)(width - agent1_x), agent1_y), new Point((ushort)(width - agent2_x), agent2_y));

            return result;
        }

        public class Player
        {
            public Point Agent1 { get; private set; }
            public Point Agent2 { get; private set; }

            public Player(Point one, Point two)
            {
                Agent1 = one;
                Agent2 = two;
            }
        }

        public ColoredBoard GetNewColoredBoard()
        {
            int count = Width * Height;
            if (count < 64)
                return new ColoredBoardSmall((ushort)Width, (ushort)Height);
            else if (Width < 16 && Height < 16)
                return new ColoredBoardSmallBigger((ushort)Width, (ushort)Height);
            else if (Width < 32 && Height < 32)
                return new ColoredBoardNormalSmaller((ushort)Width, (ushort)Height);
            else if (Width < 64 && Height < 64)
                return new ColoredBoardNormal((ushort)Width, (ushort)Height);
            //else
            //    return new ColoredBoardBig((ushort)Width, (ushort)Height);

            throw new NotSupportedException();
        }
    }
}
