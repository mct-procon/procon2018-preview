using AngryBee.Boards;
using System;
using System.Collections.Generic;
using System.Text;
#if FALSE
namespace AngryBee.AI
{
    public class AI_SingleFor
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        public int ends = 0;

        public Player Begin(int deepness, BoardSetting setting, ColoredBoardSmallBigger MeBoard, ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            (int DestX, int DestY)[] WayEnumerator = { (0, -1), (1, 0), (0, 1), (-1, 0), (1, 1), (1, -1), (-1, 1), (-1, -1) };

            Player reMe = Me;
            
            reMe.Agent1 += Search(1, deepness, WayEnumerator, MeBoard, EnemyBoard, Me, Enemy, setting.ScoreBoard, (100, 100)).Item5;
            reMe.Agent2 += Search(2, deepness, WayEnumerator, MeBoard, EnemyBoard, reMe, Enemy, setting.ScoreBoard, (100, 100)).Item5;
            
            return reMe;
        }

        Tuple<int, Player, ColoredBoardSmallBigger, ColoredBoardSmallBigger, (int,int)> Search(int num,int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, in sbyte[,] ScoreBoard, (int,int) first)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(ScoreBoard, EnemyBoard, 0), Me, MeBoard, EnemyBoard, first);
            }

            Tuple<int, Player, ColoredBoardSmallBigger, ColoredBoardSmallBigger, (int,int)> BestWay = Tuple.Create<int,Player,ColoredBoardSmallBigger,ColoredBoardSmallBigger,(int,int)>(-100000, Me, MeBoard, EnemyBoard,(0,0));
            for (int i = 0; i < WayEnumerator.Length; ++i)
            {
                Player newMe = Me;

                if (num == 1)
                {
                    newMe.Agent1 += WayEnumerator[i];

                    var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                    if (!movable.IsMovable) continue;
                    if (EnemyBoard[newMe.Agent1])
                    {
                        var newEnBoard = EnemyBoard;
                        newEnBoard[newMe.Agent1] = false;
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, newEnBoard, Me, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, newEnBoard, Me, Enemy, ScoreBoard, first), BestWay);
                    }
                    else if (MeBoard[newMe.Agent1])
                    {
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, first), BestWay);
                    }
                    else
                    {
                        var newMeBoard = MeBoard;
                        newMeBoard[newMe.Agent1] = true;
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, first), BestWay);
                    }
                }
                else
                {
                    newMe.Agent2 += WayEnumerator[i];

                    var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                    if (!movable.IsMovable) continue;
                    if (EnemyBoard[newMe.Agent2])
                    {
                        var newEnBoard = EnemyBoard;
                        newEnBoard[newMe.Agent2] = false;
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, newEnBoard, Me, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, newEnBoard, Me, Enemy, ScoreBoard, first), BestWay);
                    }
                    else if (MeBoard[newMe.Agent2])
                    {
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, MeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, first), BestWay);
                    }
                    else
                    {
                        var newMeBoard = MeBoard;
                        newMeBoard[newMe.Agent2] = true;
                        if (first == (100, 100)) BestWay = Better(Search(num, deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, WayEnumerator[i]), BestWay);
                        else BestWay = Better(Search(num, deepness - 1, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, ScoreBoard, first), BestWay);
                    }
                }

            }
            return BestWay;

        }

        Tuple<int, Player, ColoredBoardSmallBigger, ColoredBoardSmallBigger, (int,int)> Better(Tuple<int, Player, ColoredBoardSmallBigger, ColoredBoardSmallBigger, (int,int)> res, Tuple<int, Player, ColoredBoardSmallBigger, ColoredBoardSmallBigger, (int,int)> BWay)
        {
            if (BWay.Item1 <= res.Item1) return res;
            return BWay;
        }

    }
}
#endif