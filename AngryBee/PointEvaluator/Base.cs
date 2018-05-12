using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.PointEvaluator
{
    public abstract class Base
    {
        public abstract int Calculate(sbyte[,] ScoreBoard, in Boards.ColoredBoardSmallBigger Painted, int Turn);
    }
}
