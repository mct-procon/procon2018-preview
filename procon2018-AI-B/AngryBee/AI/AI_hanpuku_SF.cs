using AngryBee.Boards;
using MCTProcon29Protocol;
using MCTProcon29Protocol.Methods;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.AI
{
    public class AI_hanpuku_SF : MCTProcon29Protocol.AIFramework.AIBase
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
        private DP[] dp1 = new DP[100];
        private DP[] dp2 = new DP[100];
        int cnt;

        protected override void Solve()
        {
            (int DestX, int DestY)[] WayEnumerator = { (1, 1), (1, -1), (-1, 1), (-1, -1), (0, 1), (-1, 0), (1, 0), (0, -1) };

            DP syoki = new DP();
            for (int i = 1; i < 100; ++i)
            {
                dp1[i] = syoki;
                dp2[i] = syoki;
            }
            cnt = 1;

            var Me = new Player(MyAgent1, MyAgent2);
            var Enemy = new Player(EnemyAgent1, EnemyAgent2);

            while (!CancellationToken.IsCancellationRequested)
            {
                Player BestWay = Me;

                Search_A1(cnt, WayEnumerator, MyBoard, EnemyBoard, Me, Enemy, int.MinValue, int.MaxValue, ScoreBoard);
                BestWay.Agent1 = Me.Agent1 + dp1[cnt].Ag1Way;

                var nMe = Me;
                nMe.Agent1 = BestWay.Agent1;
                var nMeBoard = MyBoard;
                var nEnBoard = EnemyBoard;
                var movable = Checker.MovableCheck(MyBoard, EnemyBoard, nMe, Enemy);
                if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                {
                    nEnBoard[nMe.Agent1] = false;
                    nMe.Agent1 = Me.Agent1;
                }
                else
                    nMeBoard[nMe.Agent1] = true;

                Search_A2(cnt, WayEnumerator, nMeBoard, nEnBoard, nMe, Enemy, int.MinValue, int.MaxValue, ScoreBoard);
                BestWay.Agent2 = Me.Agent2 + dp2[cnt].Ag2Way;

                movable = Checker.MovableCheck(nMeBoard, nEnBoard, BestWay, Enemy);
                if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                {
                    nEnBoard[nMe.Agent2] = false;
                    nMe.Agent2 = Me.Agent2;
                }
                else
                    nMeBoard[nMe.Agent2] = true;
                SolverResult = new Decided(new VelocityPoint(dp1[cnt].Ag1Way.Item1, dp1[cnt].Ag1Way.Item2), new VelocityPoint(dp2[cnt].Ag2Way.Item1, dp2[cnt].Ag2Way.Item2));
                Log("[SOLVER] deepness = {0}", cnt);
                cnt++;
            }

        }

        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> Search_A1(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), MeBoard, EnemyBoard, Me, Enemy);
            }
            if (CancellationToken.IsCancellationRequested)
            {
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), MeBoard, EnemyBoard, Me, Enemy);
            }


            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> result = Tuple.Create(alpha, new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger(), Me, Enemy);
            if (alpha == int.MinValue && dp1[deepness].score != int.MinValue)
            {
                var newMeBoard = MeBoard;
                Player newMe = Me;
                newMe.Agent1 += dp1[deepness].Ag1Way;
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

                        result = Search_A1(deepness - 1, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                    else
                    {
                        newMeBoard[newMe.Agent1] = true;

                        result = Search_A1(deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                }

            }
            for (int i = 0; i < WayEnumerator.Length; ++i)
            {
                Player newMe = Me;
                newMe.Agent1 += WayEnumerator[i];

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

                    cache = Search_A1(deepness - 1, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                }
                else
                {
                    newMeBoard[newMe.Agent1] = true;

                    cache = Search_A1(deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                }

                if (result.Item1 < cache.Item1)
                {
                    result = cache;
                    if (deepness == cnt)
                    {
                        dp1[deepness].score = result.Item1;
                        dp1[deepness].Ag1Way = WayEnumerator[i];
                    }
                }

                if (result.Item1 >= beta)
                {
                    return result;
                }
            }
            return result;
        }

        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> Search_A2(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), MeBoard, EnemyBoard, Me, Enemy);
            }
            if (CancellationToken.IsCancellationRequested)
            {
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), MeBoard, EnemyBoard, Me, Enemy);
            }


            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> result = Tuple.Create(alpha, new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger(), Me, Enemy);
            if (alpha == int.MinValue && dp2[deepness].score != int.MinValue)
            {
                var newMeBoard = MeBoard;
                Player newMe = Me;
                newMe.Agent2 += dp2[deepness].Ag2Way;
                var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);
                if (movable.IsMovable)
                {
                    if (movable.IsEraseNeeded)
                    {
                        var newEnBoard = EnemyBoard;

                        if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent2] = false;
                            newMe.Agent2 = Me.Agent2;
                        }
                        else
                            newMeBoard[newMe.Agent2] = true;

                        result = Search_A2(deepness - 1, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                    else
                    {
                        newMeBoard[newMe.Agent2] = true;

                        result = Search_A2(deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                    }
                }

            }
            for (int i = 0; i < WayEnumerator.Length; ++i)
            {
                Player newMe = Me;
                newMe.Agent2 += WayEnumerator[i];

                var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                if (!movable.IsMovable) continue;

                Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> cache = null;
                var newMeBoard = MeBoard;

                if (movable.IsEraseNeeded)
                {
                    var newEnBoard = EnemyBoard;

                    if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                    {
                        newEnBoard[newMe.Agent2] = false;
                        newMe.Agent2 = Me.Agent2;
                    }
                    else
                        newMeBoard[newMe.Agent2] = true;

                    cache = Search_A2(deepness - 1, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                }
                else
                {
                    newMeBoard[newMe.Agent2] = true;

                    cache = Search_A2(deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, beta, ScoreBoard);
                }

                if (result.Item1 < cache.Item1)
                {
                    result = cache;
                    if (deepness == cnt)
                    {
                        dp2[deepness].score = result.Item1;
                        dp2[deepness].Ag2Way = WayEnumerator[i];
                    }
                }

                if (result.Item1 >= beta)
                {
                    return result;
                }
            }
            return result;
        }

        protected override void EndGame(GameEnd end)
        {
        }
    }
}
