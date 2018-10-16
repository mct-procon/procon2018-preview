using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameInterface.Cells;

namespace GameInterface
{
    static class ScoreCalculator
    {
        private static int[,] areaStateSearchMap; //未探索 -1 外側 0 内側 1 自陣 2
        private static int height;
        private static int width;
        private static readonly int[] DirectionX = new int[] { 1, 0, -1, 0 };
        private static readonly int[] DirectionY = new int[] { 0, 1, 0, -1 };
        private static bool[,] isSearched;
        public static int CalcScore(int playerNum, int height_, int width_, Cell[,] cells)
        {
            int score = 0;
            height = height_;
            width = width_;
            var state = playerNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cellData = cells[i, j];
                    if (cellData.AreaState_ == state)
                        score += cellData.Score;
                }
            }

            areaStateSearchMap = new int[width, height]; //-1.未探索 0.外側 1.内側 2.自陣
                                                         //囲まれた領域のポイント
                                                         //自陣判定を先にしておく
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (state == cells[j, k].AreaState_)
                        areaStateSearchMap[j, k] = 2;
                    else areaStateSearchMap[j, k] = -1;
                }
            }
            //外側、内側の判定をする
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    isSearched = new bool[width, height]; 
                    if (areaStateSearchMap[j, k] != -1||j==3) continue;
                    CheckIsInside(j, k);
                }
            }

            //内側のものは絶対値を加算する
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (areaStateSearchMap[j, k] == 1)
                        score += Math.Abs(cells[j, k].Score);
                }
            }
            return score;
        }

        private static bool CheckIsInside(int x, int y)
        {
            if (areaStateSearchMap[x, y] == 2) return true;
            isSearched[x, y] = true;
            for (int i = 0; i < 4; i++)
            {
                int ny = y + DirectionY[i], nx = x + DirectionX[i];
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) return false;
                if (isSearched[nx, ny]) continue;
                if (!CheckIsInside(nx,ny))
                {
                    return false;
                }
            }
            areaStateSearchMap[x, y] = 1;
            return true;
        }
    }
}
