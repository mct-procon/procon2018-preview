using AngryBee.Boards;
using MCTProcon29Protocol;
using MCTProcon29Protocol.Methods;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.AI
{
    class AI_outside : MCTProcon29Protocol.AIFramework.AIBase
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        (int, int) nextWay1 = (-1, -1), nextWay2 = (1, 1);


        protected override void Solve()
        {
            var res = Search(MyBoard, EnemyBoard, new Player(MyAgent1, MyAgent2), new Player(EnemyAgent1, EnemyAgent2));
            SolverResult = new Decided(new VelocityPoint((int)res.Agent1.X - (int)MyAgent1.X, (int)res.Agent1.Y - (int)MyAgent1.Y), new VelocityPoint((int)res.Agent2.X - (int)MyAgent2.X, (int)res.Agent2.Y - (int)MyAgent2.Y));
        }

        Player Search(ColoredBoardSmallBigger MeBoard, ColoredBoardSmallBigger EnemyBoard, Player Me, Player Enemy)
        {
            
            var beforeMe = Me;
            //Agent1

            if (beforeMe.Agent1.X == 0 && beforeMe.Agent1.Y != 0)
            {
                Me.Agent1 += (0, -1);
                if (MeBoard[Me.Agent1])
                {
                    Me.Agent1 += (1, 0);
                    nextWay1 = (-1, -1);
                }
            }
            if (beforeMe.Agent1.Y == 0 && beforeMe.Agent1.X != MeBoard.Width - 1)
            {
                Me.Agent1 += (1, 0);
                if (MeBoard[Me.Agent1])
                {
                    Me.Agent1 += (0, 1);
                    nextWay1 = (1, -1);
                }
            }
            if (beforeMe.Agent1.X == MeBoard.Width - 1 && beforeMe.Agent1.Y != MeBoard.Height - 1)
            {
                Me.Agent1 += (0, 1);
                if (MeBoard[Me.Agent1])
                {
                    Me.Agent1 += (-1, 0);
                    nextWay1 = (1, 1);
                }
            }
            if (beforeMe.Agent1.Y == MeBoard.Height - 1 && beforeMe.Agent1.X != 0)
            {
                Me.Agent1 += (-1, 0);
                if (MeBoard[Me.Agent1])
                {
                    Me.Agent1 += (0, -1);
                    nextWay1 = (-1, 1);
                }
            }
            if (beforeMe.Agent1 == Me.Agent1)
            {
                Me.Agent1 += nextWay1;
                if (MeBoard[Me.Agent1])
                {
                    if ((Me.Agent1.X == 0 || Me.Agent1.X == MeBoard.Width - 1) && (Me.Agent1.Y == 0 || Me.Agent1.Y == MeBoard.Height - 1)) { }
                    else
                    {
                        if (Me.Agent1.X == 0) Me.Agent1 += (1, 0);
                        if (Me.Agent1.X == MeBoard.Width - 1) Me.Agent1 += (-1, 0);
                        if (Me.Agent1.Y == 0) Me.Agent1 += (0, 1);
                        if (Me.Agent1.Y == MeBoard.Height - 1) Me.Agent1 += (0, -1);
                    }
                }
            }

            //Agent2
            if (beforeMe.Agent2.X == 0 && beforeMe.Agent2.Y != 0)
            {
                Me.Agent2 += (0, -1);
                if (MeBoard[Me.Agent2])
                {
                    Me.Agent2 += (1, 0);
                    nextWay2 = (-1, -1);
                }
            }
            if (beforeMe.Agent2.Y == 0 && beforeMe.Agent2.X != MeBoard.Width - 1)
            {
                Me.Agent2 += (1, 0);
                if (MeBoard[Me.Agent2])
                {
                    Me.Agent2 += (0, 1);
                    nextWay2 = (1, -1);
                }
            }
            if (beforeMe.Agent2.X == MeBoard.Width - 1 && beforeMe.Agent2.Y != MeBoard.Height - 1)
            {
                Me.Agent2 += (0, 1);
                if (MeBoard[Me.Agent2])
                {
                    Me.Agent2 += (-1, 0);
                    nextWay2 = (1, 1);
                }
            }
            if (beforeMe.Agent2.Y == MeBoard.Height - 1 && beforeMe.Agent2.X != 0)
            {
                Me.Agent2 += (-1, 0);
                if (MeBoard[Me.Agent2])
                {
                    Me.Agent2 += (0, -1);
                    nextWay2 = (-1, 1);
                }
            }
            if (beforeMe.Agent2 == Me.Agent2)
            {
                Me.Agent2 += nextWay2;
                if (MeBoard[Me.Agent2])
                {
                    if ((Me.Agent2.X == 0 || Me.Agent2.X == MeBoard.Width - 1) && (Me.Agent2.Y == 0 || Me.Agent2.Y == MeBoard.Height - 1)) { }
                    else
                    {
                        if (Me.Agent2.X == 0) Me.Agent2 += (1, 0);
                        if (Me.Agent2.X == MeBoard.Width - 1) Me.Agent2 += (-1, 0);
                        if (Me.Agent2.Y == 0) Me.Agent2 += (0, 1);
                        if (Me.Agent2.Y == MeBoard.Height - 1) Me.Agent2 += (0, -1);
                    }
                }
            }

            return Me;
        }

        protected override void EndGame(GameEnd end)
        {
        }
    }
}
