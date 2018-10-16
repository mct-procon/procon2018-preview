using System;
using System.Collections.Generic;
using System.Text;
using AngryBee.Boards;
using System.Runtime.Intrinsics.X86;
using MCTProcon29Protocol;

namespace AngryBee.PointEvaluator
{

    //通常の計算に加え、自分が囲んでいる囲みの数*50点を加算する。
    public class PrioritySurrond : Base
    {
        readonly int[] DistanceX = { 0, 1, 0, -1 };
        readonly int[] DistanceY = { 1, 0, -1, 0 };
        public override int Calculate(sbyte[,] ScoreBoard, in ColoredBoardSmallBigger Painted, int Turn)
        {
            ColoredBoardSmallBigger checker = new ColoredBoardSmallBigger(Painted.Width, Painted.Height);
            int result = 0;
            uint width = Painted.Width;
            uint height = Painted.Height;
            for (uint x = 0; x < width; ++x)
                for (uint y = 0; y < height; ++y)
                {
                    if (Painted[x, y])
                    {
                        result += ScoreBoard[x, y];
                        checker[x, y] = true;
                    }
                }

            result += CountSurrounded(checker, width, height) * 50;

            BadSpaceFill(ref checker, width, height);

            for (uint x = 0; x < width; ++x)
                for (uint y = 0; y < height; ++y)
                    if (!checker[x, y])
                        result += Math.Abs(ScoreBoard[x, y]);

            return result;
        }

        //uint[] myStack = new uint[1024];	//x, yの順で入れる. y, xの順で取り出す. width * height以上のサイズにする.
        public unsafe void BadSpaceFill(ref ColoredBoardSmallBigger Checker, uint width, uint height)
        {
            unchecked
            {
                Point* myStack = stackalloc Point[12 * 12];

                Point point;
                uint x, y, searchTo = 0, myStackSize = 0;

                searchTo = height - 1;
                for (x = 0; x < width; x++)
                {
                    if (!Checker[x, 0])
                    {
                        myStack[myStackSize++] = new Point(x, 0);
                        Checker[x, 0] = true;
                    }
                    if (!Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }
                }

                searchTo = width - 1;
                for (y = 0; y < height; y++)
                {
                    if (!Checker[0, y])
                    {
                        myStack[myStackSize++] = new Point(0, y);
                        Checker[0, y] = true;
                    }
                    if (!Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }
                }

                while (myStackSize > 0)
                {
                    point = myStack[--myStackSize];
                    x = point.X;
                    y = point.Y;

                    //左方向
                    searchTo = x - 1;
                    if (searchTo < width && !Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }

                    //下方向
                    searchTo = y + 1;
                    if (searchTo < height && !Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }

                    //右方向
                    searchTo = x + 1;
                    if (searchTo < width && !Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }

                    //上方向
                    searchTo = y - 1;
                    if (searchTo < height && !Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }

                }
            }
        }

        int CountSurrounded(ColoredBoardSmallBigger Checker, uint width, uint height)
        {
            int count = 0;
            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    if (!Checker[x, y])
                    {
                        if (FillChecker(ref Checker, x, y, width, height)) count++;
                    }
                }
            }
            Console.WriteLine(count.ToString());
            return count;
        }

        bool FillChecker(ref ColoredBoardSmallBigger Checker, uint x, uint y, in uint Width, in uint Height)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            if (Checker[x, y]) return true;
            Checker[x, y] = true;
            for (int i = 0; i < 4; i++)
            {
                if (!FillChecker(ref Checker, (uint)(x + DistanceX[i]), (uint)(y + DistanceY[i]), Width, Height)) return false;
            }
            return true;
        }
    }
}
