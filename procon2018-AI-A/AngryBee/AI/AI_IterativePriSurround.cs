using AngryBee.Boards;
using System;
using System.Collections.Generic;
using System.Text;
using MCTProcon29Protocol.Methods;
using MCTProcon29Protocol;

namespace AngryBee.AI
{
    /// <summary>
    /// 反復深化法とminimax法を用いて、最高得点をとれるパターンを計算するAI。
    /// 評価関数の計算に[自軍の囲んでいる陣地数*50]を加えているので、ひたすら囲みを増やそうと動く。(はず)
    /// </summary>
    public class AI_IterativePriSurround : MCTProcon29Protocol.AIFramework.AIBase
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();
        PointEvaluator.PrioritySurrond PointEvaluatorPriSurround = new PointEvaluator.PrioritySurrond();

        private class DP
        {
            public int score = int.MinValue;
            public VelocityPoint Ag1Way = (0, 0);
            public VelocityPoint Ag2Way = (0, 0);
        }

        private DP[] dp = new DP[100];
        private int deepness;

        public void IterativePriSurround(sbyte[,] ScoreBoard, ColoredBoardSmallBigger MeBoard, ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            VelocityPoint[] WayEnumerator = { (1, 1), (1, -1), (-1, 1), (-1, -1), (0, 1), (-1, 0), (1, 0), (0, -1) };

            for (int i = 1; i < 100; ++i)
            {
                dp[i] = new DP();
            }
            deepness = 1;

            Decided BestWay = new Decided();
            while (deepness < 100)
            {
                Max(deepness, WayEnumerator, MeBoard, EnemyBoard, Me, Enemy, int.MinValue, int.MaxValue, ScoreBoard);
                if (!CancellationToken.IsCancellationRequested)
                {
                    BestWay.MeAgent1 = dp[deepness].Ag1Way;
                    BestWay.MeAgent2 = dp[deepness].Ag2Way;
                    deepness++;
                }
                else break;
            }
            SolverResult = BestWay;
        }

        int Max(int deepness, in VelocityPoint[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            if (CancellationToken.IsCancellationRequested) { return 0; }

            if (deepness == 0)
            {
                return PointEvaluatorPriSurround.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0);
            }

            int result = alpha;
            if (alpha == int.MinValue && dp[deepness].score != int.MinValue)
            {
                Player newMe = Me;
                newMe.Agent1 += dp[deepness].Ag1Way;
                newMe.Agent2 += dp[deepness].Ag2Way;
                var moveResult = Move(MeBoard, EnemyBoard, newMe, Enemy);

                if (moveResult != null)
                {
                    var newMeBoard = moveResult.Item1;
                    var newEnBoard = moveResult.Item2;
                    newMe = moveResult.Item3;
                    var newEnemy = moveResult.Item4;
                    result = Max(deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result, beta, ScoreBoard);
                }

            }
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    if (CancellationToken.IsCancellationRequested) { return 0; }

                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    var moveResult = Move(MeBoard, EnemyBoard, newMe, Enemy);

                    if (moveResult == null) continue;

                    int cache = 0;
                    var newMeBoard = moveResult.Item1;
                    var newEnBoard = moveResult.Item2;
                    newMe = moveResult.Item3;
                    var newEnemy = moveResult.Item4;

                    cache = Max(deepness - 1, WayEnumerator, newMeBoard, newEnBoard, newMe, newEnemy, result, beta, ScoreBoard);

                    if (result < cache)
                    {
                        result = Math.Max(result, cache);
                        if (deepness == this.deepness)
                        {
                            dp[deepness].score = result;
                            dp[deepness].Ag1Way = WayEnumerator[i];
                            dp[deepness].Ag2Way = WayEnumerator[m];
                        }
                    }

                    if (result >= beta)
                    {
                        return result;
                    }

                }
            return result;
        }

        int Mini(int deepness, in VelocityPoint[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, in sbyte[,] ScoreBoard)
        {
            deepness--;
            if (CancellationToken.IsCancellationRequested)
            {
                return 0;
            }


            int result = beta;
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    if (WayEnumerator[i] == WayEnumerator[m])
                        continue;

                    Player newEnemy = Enemy;
                    newEnemy.Agent1 += WayEnumerator[i];
                    newEnemy.Agent2 += WayEnumerator[m];

                    var moveResult = Move(EnemyBoard, MeBoard, newEnemy, Me);

                    if (moveResult == null) continue;

                    int cache = 0;

                    var newEnBoard = moveResult.Item1;
                    var newMeBoard = moveResult.Item2;
                    newEnemy = moveResult.Item3;
                    var newMe = moveResult.Item4;

                    cache = Max(deepness, WayEnumerator, newMeBoard, newEnBoard, Me, newEnemy, alpha, result, ScoreBoard);

                    result = Math.Min(result, cache);

                    if (result <= alpha)
                    {
                        return result;
                    }

                }
            return result;
        }

        Tuple<ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player> Move(ColoredBoardSmallBigger meBoard, ColoredBoardSmallBigger enemyBoard, Player me, Player enemy)
        {
            var movable = Checker.MovableCheck(meBoard, enemyBoard, me, enemy);

            if (!movable.IsMovable) return null;

            if (movable.IsEraseNeeded)
            {

                if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                {
                    enemyBoard[me.Agent1] = false;
                    me.Agent1 = me.Agent1;
                }
                else
                    meBoard[me.Agent1] = true;

                if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                {
                    enemyBoard[me.Agent2] = false;
                    me.Agent2 = me.Agent2;
                }
                else
                    meBoard[me.Agent2] = true;

            }
            else
            {
                meBoard[me.Agent1] = true;
                meBoard[me.Agent2] = true;
            }
            return new Tuple<ColoredBoardSmallBigger, ColoredBoardSmallBigger, Player, Player>(meBoard, enemyBoard, me, enemy);
        }

        protected override void EndGame(GameEnd end)
        {
        }

        protected override void Solve()
        {
            IterativePriSurround(ScoreBoard, MyBoard, EnemyBoard, new Player(MyAgent1, MyAgent2), new Player(EnemyAgent1, EnemyAgent2));
        }
    }
}