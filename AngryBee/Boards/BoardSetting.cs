﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Boards
{
    public class BoardSetting
    {
        public sbyte[,] ScoreBoard { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private BoardSetting() { }

        public static (BoardSetting setting, Player me, Player enemy) Generate(byte height = 12, byte width = 12)
        {
            if ((width & 0b1) == 1)
                throw new ArgumentException("width must be an odd number.", nameof(width));

            BoardSetting result = new BoardSetting();
            result.Width = width;
            result.Height = height;
            result.ScoreBoard = new sbyte[height, width];

            Random rand = new Random(114514);

            int widthDiv2 = width / 2;

            for (int y = 0; y < height; ++y)
                for (int x = 0; x < widthDiv2; ++x)
                {
                    int value = rand.Next(10);
                    value = (value == 0) ? - rand.Next(16) : rand.Next(16);
                    sbyte value_s = (sbyte)value;
                    result.ScoreBoard[x, y] = value_s;
                    result.ScoreBoard[result.ScoreBoard.GetLength(1) - 1 - x, y] = value_s;
                }

            int heightDiv2 = height / 2;

            ushort agent1_y = (ushort)rand.Next(heightDiv2);
            ushort agent2_y = (ushort)(rand.Next(heightDiv2) + heightDiv2);

            int agent1_x = rand.Next(widthDiv2);
            int agent2_x = rand.Next(widthDiv2);

            return (
                result,
                new Player(new Point((ushort)agent1_x, agent1_y), new Point((ushort)agent2_x, agent2_y)),
                new Player(new Point((ushort)(width - agent1_x - 1), agent1_y), new Point((ushort)(width - agent2_x - 1), agent2_y))
                );
        }

        [Obsolete("It is slower.")]
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
