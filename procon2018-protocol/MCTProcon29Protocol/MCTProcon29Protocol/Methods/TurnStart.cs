using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class TurnStart
    {
        [Key(0)]
        public byte Turn { get; set; }

        [Key(1)]
        public int WaitMiliSeconds { get; set; }

        [Key(2)]
        public Point MeAgent1 { get; set; }

        [Key(3)]
        public Point MeAgent2 { get; set; }

        [Key(4)]
        public Point EnemyAgent1 { get; set; }

        [Key(5)]
        public Point EnemyAgent2 { get; set; }

        [Key(6)]
        public ColoredBoardSmallBigger MeColoredBoard { get; set; }

        [Key(7)]
        public ColoredBoardSmallBigger EnemyColoredBoard { get; set; }

        public TurnStart(byte turn, int waitMiliSecs, Point meAgent1, Point meAgent2, Point enemyAgent1, Point enemyAgent2, in ColoredBoardSmallBigger meColoredBoard, in ColoredBoardSmallBigger enemyColoredBoard)
        {
            Turn = turn;
            WaitMiliSeconds = waitMiliSecs;
            MeAgent1 = meAgent1;
            MeAgent2 = meAgent2;
            EnemyAgent1 = enemyAgent1;
            EnemyAgent2 = enemyAgent2;
            MeColoredBoard = meColoredBoard;
            EnemyColoredBoard = enemyColoredBoard;
        }

        // DO NOT ERASE
        public TurnStart() { }
    }
}
