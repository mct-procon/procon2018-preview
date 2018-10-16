using MCTProcon29Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.PointEvaluator
{
    public abstract class Base
    {
        public abstract int Calculate(sbyte[,] ScoreBoard, in ColoredBoardSmallBigger Painted, int Turn);
    }
}
