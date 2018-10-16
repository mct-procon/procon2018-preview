using AngryBee.Boards;
using MCTProcon29Protocol;
using System;
using System.Collections.Generic;
using System.Text;
#if FALSE
namespace AngryBee.AI
{
    public class AI_hanpuku_a
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        public int ends = 0;

        private class DP
        {
            public int score = int.MinValue;
            public (int, int) Ag1Way = (0, 0);
            public (int, int) Ag2Way = (0, 0);
        }
        private DP[] dp = new DP[100];
        int cnt;

        public Player Begin(int deepness, BoardSetting setting, ColoredBoardSmallBigger MeBoard, ColoredBoardSmallBigger EnemyBoard, Player Me, in Player Enemy)
        {
            (int DestX, int DestY)[] WayEnumerator = { (1, 1), (1, -1), (-1, 1), (-1, -1), (0, 1), (-1, 0), (1, 0), (0, -1) };

            DP syoki = new DP();
            for (int i = 1; i < 100; ++i)
            {
                dp[i] = syoki;
            }
            cnt = 1;

            return Search(deepness, WayEnumerator, MeBoard, EnemyBoard, Me, Enemy, setting.ScoreBoard);
        }


        Player Search(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, in sbyte[,] ScoreBoard)
        {
            Player BestWay = Me;
            while (deepness >= cnt)
            {
                Max(cnt, WayEnumerator, MeBoard, EnemyBoard, Me, Enemy, int.MinValue, int.MaxValue, ScoreBoard);
                BestWay.Agent1 = Me.Agent1 + dp[cnt].Ag1Way;
                BestWay.Agent2 = Me.Agent2 + dp[cnt].Ag2Way;
                cnt++;
            }

            return BestWay;
        }



        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> Max(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), MeBoard, EnemyBoard, Me, Enemy);
            }


            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> result = Tuple.Create(alpha, new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger(), Me, Enemy);
            if (alpha == int.MinValue && dp[deepness].score != int.MinValue)
            {
                var newMeBoard = MeBoard;
                Player newMe = Me;
                newMe.Agent1 += dp[deepness].Ag1Way;
                newMe.Agent2 += dp[deepness].Ag2Way;
                var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);
                if (movable.IsMovable)
                {
                    if (movable.IsEraseNeeded)
                    {
                        var newEnBoard = EnemyBoard;

                        if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent1] = false;
                            newMe.Agent1 = Me.Agent1;
                        }
                        else
                            newMeBoard[newMe.Agent1] = true;

                        if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent2] = false;
                            newMe.Agent2 = Me.Agent2;
                        }
                        else
                            newMeBoard[newMe.Agent2] = true;

                        result = Mini(deepness, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                    else
                    {
                        newMeBoard[newMe.Agent1] = true;
                        newMeBoard[newMe.Agent2] = true;

                        result = Mini(deepness, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                }

            }
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {

                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                    if (!movable.IsMovable) continue;

                    Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> cache = null;
                    var newMeBoard = MeBoard;

                    if (movable.IsEraseNeeded)
                    {
                        var newEnBoard = EnemyBoard;

                        if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent1] = false;
                            newMe.Agent1 = Me.Agent1;
                        }
                        else
                            newMeBoard[newMe.Agent1] = true;

                        if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent2] = false;
                            newMe.Agent2 = Me.Agent2;
                        }
                        else
                            newMeBoard[newMe.Agent2] = true;

                        cache = Mini(deepness, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                    else
                    {
                        newMeBoard[newMe.Agent1] = true;
                        newMeBoard[newMe.Agent2] = true;

                        cache = Mini(deepness, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }

                    if (result.Item1 < cache.Item1)
                    {
                        result = cache;
                        if (deepness == cnt)
                        {
                            dp[deepness].score = result.Item1;
                            dp[deepness].Ag1Way = WayEnumerator[i];
                            dp[deepness].Ag2Way = WayEnumerator[m];
                        }
                    }

                    if (result.Item1 >= beta)
                    {
                        return result;
                    }

                }
            return result;
        }

        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> Mini(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            deepness--;

            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> result = Tuple.Create(beta, new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger(), Me, Enemy);
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    if (WayEnumerator[i] == WayEnumerator[m])
                        continue;

                    Player newEnemy = Enemy;
                    newEnemy.Agent1 += WayEnumerator[i];
                    newEnemy.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(EnemyBoard, MeBoard, newEnemy, Me);

                    if (!movable.IsMovable) continue;

                    Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> cache = null;
                    var newEnBoard = EnemyBoard;

                    if (movable.IsEraseNeeded)
                    {
                        var newMeBoard = MeBoard;

                        if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                        {
                            newMeBoard[newEnemy.Agent1] = false;
                            newEnemy.Agent1 = Enemy.Agent1;
                        }
                        else
                            newEnBoard[newEnemy.Agent1] = true;

                        if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                        {
                            newMeBoard[newEnemy.Agent2] = false;
                            newEnemy.Agent2 = Enemy.Agent2;
                        }
                        else
                            newEnBoard[newEnemy.Agent2] = true;

                        cache = Max(deepness, WayEnumerator, newMeBoard, newEnBoard, Me, newEnemy, alpha, result.Item1, ScoreBoard);
                    }
                    else
                    {
                        newEnBoard[newEnemy.Agent1] = true;
                        newEnBoard[newEnemy.Agent2] = true;
                        cache = Max(deepness, WayEnumerator, MeBoard, newEnBoard, Me, newEnemy, alpha, result.Item1, ScoreBoard);
                    }

                    if (result.Item1 > cache.Item1)
                    {
                        result = cache;
                    }

                    if (result.Item1 <= alpha)
                    {
                        return result;
                    }

                }

            return result;
        }


    }
}
#endif