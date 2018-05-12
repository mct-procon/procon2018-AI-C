using System;
using System.Collections.Generic;
using System.Text;
using AngryBee.Boards;

namespace AngryBee.PointEvaluator
{
    public class Normal : Base
    {
        public override int Calculate(sbyte[,] ScoreBoard, in ColoredBoardSmallBigger Painted, int Turn)
        {
            ColoredBoardSmallBigger checker = new ColoredBoardSmallBigger(Painted.Width, Painted.Height);
            int result = 0;
            uint width = Painted.Width;
            uint height = Painted.Height;
            for (uint x = 0; x < Painted.Width; ++x)
                for (uint y = 0; y < Painted.Height; ++y)
                {
                    int cache = Calculate(ScoreBoard, Painted, ref checker, x, y, width, height);
                    if (cache != int.MinValue)
                        result += cache;
                }

            return result;
        }

        public int Calculate(sbyte[,] ScoreBoard, in ColoredBoardSmallBigger Painted, ref ColoredBoardSmallBigger Checker, uint x, uint y, uint width, uint height)
        {
            unchecked
            {
                if (Checker[x, y]) return 0;

                Checker[x, y] = true;

                if (Painted[x, y]) return ScoreBoard[x, y];

                uint Right = x + 1u;
                uint Left = x - 1u;

                if (Right >= width || Left >= width)
                    return int.MinValue;

                uint Bottom = y + 1u;
                uint Top = y - 1u;

                if (Top >= height || Bottom >= height)
                    return int.MinValue;

                int result = 0;

                if (!Painted[Left, Top])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, Left, Top, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[x, Top])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, x, Top, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[Right, Top])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, Right, Top, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[Right, y])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, Right, y, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[Right, Bottom])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, Right, Bottom, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[x, Bottom])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, x, Bottom, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                if (!Painted[Left, Bottom])
                {
                    int cache = Calculate(ScoreBoard, Painted, ref Checker, Left, Bottom, width, height);
                    if (cache == int.MinValue)
                        return int.MinValue;
                    else
                        result += cache;
                }

                result += Math.Abs(ScoreBoard[x, y]);

                return result;
            }
        }
    }
}
