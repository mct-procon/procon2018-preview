using AngryBee.Boards;
using System;
using System.Collections.Generic;
using System.Text;
using MCTProcon29Protocol.Methods;
using MCTProcon29Protocol;

namespace AngryBee.AI
{
    public class AI_koushi : MCTProcon29Protocol.AIFramework.AIBase
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        VelocityPoint[] WayEnumerator = {(1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1) };

        private struct DP
        {
            public int Score;
            public VelocityPoint Agent1Way;
            public VelocityPoint Agent2Way;

            public void UpdateScore(int score, VelocityPoint a1, VelocityPoint a2)
            {
                if (Score < score)
                {
                    Agent1Way = a1;
                    Agent2Way = a2;
                }
            }
        }
        private DP[] dp = new DP[50];

        //public int ends = 0;

        public int StartDepth { get; set; } = 1;

        public AI_koushi(int startDepth = 1)
        {
            for (int i = 0; i < 50; ++i)
                dp[i] = new DP();
            StartDepth = startDepth;
        }

        //1ターン = 深さ2
        protected override void Solve()
        {
            for (int i = 0; i < 50; ++i)
                dp[i].Score = int.MinValue;
            int deepness = StartDepth;
            for (; ; deepness++)
            {
                var tmp = SolveSub(deepness);
                if (CancellationToken.IsCancellationRequested == false)
                    SolverResult = tmp;
                else
                    break;
                Log("[SOLVER] deepness = {0}", deepness);
            }
        }

        private Decided SolveSub(int deepness)
        {
            int alpha = int.MinValue + 1;
            int beta = int.MaxValue;
            int result = int.MinValue;

            Player Killer = new Player(new Point(114, 114), new Point(114, 114));
            var nextMe = MoveOrderling(2, ScoreBoard, MyBoard, EnemyBoard, new Player(MyAgent1, MyAgent2), new Player(EnemyAgent1, EnemyAgent2), 0);

            Decided returnValue = null;

            for (int i = 0; i < nextMe.Count; i++)
            {
                var nextMeValue = nextMe[i].Value;
                Player newMe = new Player(MyAgent1, MyAgent2);
                newMe.Agent1 += nextMeValue.Agent1;
                newMe.Agent2 += nextMeValue.Agent2;

                var movable = Checker.MovableCheck(MyBoard, EnemyBoard, newMe, new Player(EnemyAgent1, EnemyAgent2));

                if (!movable.IsMovable) continue;

                int current = 0;
                var newMeBoard = MyBoard;

                if (movable.IsEraseNeeded)
                {
                    var newEnBoard = EnemyBoard;

                    if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                    {
                        newEnBoard[newMe.Agent1] = false;
                        newMe.Agent1 = MyAgent1;
                    }
                    else
                        newMeBoard[newMe.Agent1] = true;

                    if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                    {
                        newEnBoard[newMe.Agent2] = false;
                        newMe.Agent2 = MyAgent2;
                    }
                    else
                        newMeBoard[newMe.Agent2] = true;

                    current = Mini(deepness - 1, ScoreBoard, newMeBoard, newEnBoard, newMe, new Player(EnemyAgent1, EnemyAgent2), Math.Max(result, alpha), beta, 1);
                }
                else
                {
                    newMeBoard[newMe.Agent1] = true;
                    newMeBoard[newMe.Agent2] = true;
                    current = Mini(deepness - 1, ScoreBoard, newMeBoard, EnemyBoard, newMe, new Player(EnemyAgent1, EnemyAgent2), Math.Max(result, alpha), beta, 1);
                }

                if (result < current)
                {
                    result = current;
                    dp[0].UpdateScore(result, nextMeValue.Agent1, nextMeValue.Agent2);
                    returnValue = new Decided(nextMeValue.Agent1, nextMeValue.Agent2);
                }
                if (result >= beta)
                {
                    return returnValue;
                }
            }
            return returnValue;
        }

        //Meが動く
        public int Max(int deepness, sbyte[,] ScoreBoard, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, int count)
        {
            if (deepness == 0)
            {
                //ends++;
                return PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0);
            }

            int result = int.MinValue;

            Player Killer = new Player(new Point(114, 114), new Point(114, 114));
            var nextMe = MoveOrderling(2, ScoreBoard, MeBoard, EnemyBoard, Me, Enemy, count);

            for (int i = 0; i < nextMe.Count; i++)
            {
                if (CancellationToken.IsCancellationRequested) { break; }

                var nextMeValue = nextMe[i].Value;
                Player newMe = Me;
                newMe.Agent1 += nextMeValue.Agent1;
                newMe.Agent2 += nextMeValue.Agent2;

                var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                if (!movable.IsMovable) continue;

                int current = 0;
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

                    current = Mini(deepness - 1, ScoreBoard, newMeBoard, newEnBoard, newMe, Enemy, Math.Max(result, alpha), beta, count + 1);
                }
                else
                {
                    newMeBoard[newMe.Agent1] = true;
                    newMeBoard[newMe.Agent2] = true;
                    current = Mini(deepness - 1, ScoreBoard, newMeBoard, EnemyBoard, newMe, Enemy, Math.Max(result, alpha), beta, count + 1);
                }

                if (result < current)
                {
                    result = current;
                    dp[count].UpdateScore(result, nextMeValue.Agent1, nextMeValue.Agent2);
                }
                if (result >= beta)
                {
                    return result;
                }
            }

            return result;
        }

        //Enemyが動く
        public int Mini(int deepness, sbyte[,] ScoreBoard, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta, int count)
        {
            if (deepness == 0)
            {
                //ends++;
                return PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0);
            }

            int result = int.MaxValue;

            Player Killer = new Player(new Point(114, 114), new Point(114, 114));
            var nextEnemy = MoveOrderling(1, ScoreBoard, EnemyBoard, MeBoard, Enemy, Me, count);

            for (int i = 0; i < nextEnemy.Count; i++)
            {
                if (CancellationToken.IsCancellationRequested) { break; }

                var nextEnemyValue = nextEnemy[i].Value;
                Player newEnemy = Enemy;
                newEnemy.Agent1 += nextEnemyValue.Agent1;
                newEnemy.Agent2 += nextEnemyValue.Agent2;

                var movable = Checker.MovableCheck(EnemyBoard, MeBoard, newEnemy, Me);

                if (!movable.IsMovable) continue;

                int current = 0;
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


                    current = Max(deepness - 1, ScoreBoard, newMeBoard, newEnBoard, Me, newEnemy, alpha, Math.Min(result, beta), count + 1);
                }
                else
                {
                    newEnBoard[newEnemy.Agent1] = true;
                    newEnBoard[newEnemy.Agent2] = true;
                    current = Max(deepness - 1, ScoreBoard, MeBoard, newEnBoard, Me, newEnemy, alpha, Math.Min(result, beta), count + 1);
                }

                if (result > current)
                {
                    result = current;
                    dp[count].UpdateScore(-result, nextEnemyValue.Agent1, nextEnemyValue.Agent2);
                }
                if (result <= alpha)
                    return result;
            }

            return result;
        }

        //遷移順を決める.  「この関数においては」MeBoard…手番プレイヤのボード, Me…手番プレイヤ、とします。
        //(この関数におけるMeは、Maxi関数におけるMe, Mini関数におけるEnemyです）
        //newMe[0]が最初に探索したい行き先、nextMe[1]が次に探索したい行き先…として、nextMeに「次の行き先」を入れていきます。
        //以下のルールで優先順を決めます。
        //ルール1. Killer手があれば、それを優先する。(Killer手がなければ、Killer.Agent1 = (514, 514), Killer.Agent2 = (514, 514)のように範囲外の移動先を設定すること。)
        //ルール2. 次のmoveで得られる「タイルポイント」の合計値、が大きい移動(の組み合わせ)を優先する。
        //なお、ルールはMovableChecker.csに準ずるため、現在は、「タイル除去先にもう一方のエージェントが移動することはできない」として計算しています。
        private List<KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)>> MoveOrderling(int adder, sbyte[,] ScoreBoard, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int deep)
        {
            uint width = MeBoard.Width;
            uint height = MeBoard.Height;
            List<KeyValuePair<int, (VelocityPoint, VelocityPoint)>> orderling = new List<KeyValuePair<int, (VelocityPoint, VelocityPoint)>>();

            var Killer = dp[deep].Score == int.MinValue ? new Player(new Point(114, 514), new Point(114, 514)) : new Player(Me.Agent1 + dp[deep].Agent1Way, Me.Agent2 + dp[deep].Agent2Way);

            for (int i = 0; i < WayEnumerator.Length; i+=adder)
            {
                for (int m = 0; m < WayEnumerator.Length; m+= adder)
                {
                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    int score = 0;  //優先度 (小さいほど優先度が高い）
                    if (newMe.Agent1 == Killer.Agent1 && newMe.Agent2 == Killer.Agent2) score = -100;
                    else if (newMe.Agent1.X >= width || newMe.Agent1.Y >= height) score = 100;
                    else if (newMe.Agent2.X >= width || newMe.Agent2.Y >= height) score = 100;
                    else if (newMe.Agent1 == newMe.Agent2) score = 100;
                    else if (newMe.Agent1 == Enemy.Agent1) score = 100;
                    else if (newMe.Agent1 == Enemy.Agent2) score = 100;
                    else if (newMe.Agent2 == Enemy.Agent1) score = 100;
                    else if (newMe.Agent2 == Enemy.Agent2) score = 100;
                    else
                    {
                        if (!MeBoard[newMe.Agent1.X, newMe.Agent1.Y] && !EnemyBoard[newMe.Agent1.X, newMe.Agent1.Y])
                        {
                            score += ScoreBoard[newMe.Agent1.X, newMe.Agent1.Y];
                        }
                        if (!MeBoard[newMe.Agent2.X, newMe.Agent2.Y] && !EnemyBoard[newMe.Agent2.X, newMe.Agent2.Y])
                        {
                            score += ScoreBoard[newMe.Agent2.X, newMe.Agent2.Y];
                        }
                        score = -score;
                    }
                    orderling.Add(new KeyValuePair<int, (VelocityPoint, VelocityPoint)>(score, (WayEnumerator[i], WayEnumerator[m])));
                }
            }
            orderling.Sort(impl_sorter);
            return orderling;
        }

        private int impl_sorter(KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)> a, KeyValuePair<int, (VelocityPoint Agent1, VelocityPoint Agent2)> b) => a.Key - b.Key;

        protected override int CalculateTimerMiliSconds(int miliseconds)
        {
            return miliseconds - 1000;
        }

        protected override void EndGame(GameEnd end)
        {
        }

    }
}
