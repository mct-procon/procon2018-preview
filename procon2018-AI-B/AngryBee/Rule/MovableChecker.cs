using AngryBee.Boards;
using MCTProcon29Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Rule
{
    public class MovableChecker
    {
        public MovableResult MovableCheck(in ColoredBoardSmallBigger MeField, in ColoredBoardSmallBigger EnemyField, Boards.Player Me, Boards.Player Enemy )
        {
            MovableResult result = new MovableResult();

            uint width = MeField.Width, height = MeField.Height;

            if (Me.Agent1.X >= width || Me.Agent1.Y >= height)
            {
                result.Me1 = MovableResultType.OutOfField;
                return result;
            }
            if (Me.Agent2.X >= width || Me.Agent2.Y >= height)
            {
                result.Me2 = MovableResultType.OutOfField;
                return result;
            }

            if(Me.Agent1 == Enemy.Agent1 || Me.Agent1 == Enemy.Agent2)
            {
                result.Me1 = MovableResultType.EnemyIsHere;
                return result;
            }
            if(Me.Agent2 == Enemy.Agent1 || Me.Agent2 == Enemy.Agent2)
            {
                result.Me2 = MovableResultType.EnemyIsHere;
                return result;
            }

            if (Me.Agent1 == Me.Agent2)
            {
                if (EnemyField[Me.Agent1])
                    result.Me1 = MovableResultType.EraseNeeded;
                else if (EnemyField[Me.Agent2])
                    result.Me2 = MovableResultType.EraseNeeded;

                if (result.IsEraseNeeded)
                    result.Me1 = MovableResultType.Ok;
                else
                {
                    result.Me1 = MovableResultType.EnemyIsHere;
                    result.Me2 = MovableResultType.EnemyIsHere;
                }
            }
            else
            {
                if (EnemyField[Me.Agent1])
                    result.Me1 = MovableResultType.EraseNeeded;
                if (EnemyField[Me.Agent2])
                    result.Me2 = MovableResultType.EraseNeeded;
            }
            return result;
        }

        
    }
}
