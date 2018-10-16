using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class GameInit
    {
        [Key(0)]
        public byte BoardHeight { get; set; }

        [Key(1)]
        public byte BoardWidth { get; set; }

        [Key(2)]
        public sbyte[,] Board { get; set; }

        [Key(3)]
        public Point MeAgent1 { get; set; }

        [Key(4)]
        public Point MeAgent2 { get; set; }

        [Key(5)]
        public Point EnemyAgent1 { get; set; }

        [Key(6)]
        public Point EnemyAgent2 { get; set; }

        [Key(7)]
        public byte Turns { get; set; }

        public GameInit(byte height, byte width, sbyte[,] board, Point meAgent1, Point meAgent2, Point enemyAgent1, Point enemyAgent2, byte turns)
        {
            BoardHeight = height;
            BoardWidth = width;
            Board = board;
            MeAgent1 = meAgent1;
            MeAgent2 = meAgent2;
            EnemyAgent1 = enemyAgent1;
            EnemyAgent2 = enemyAgent2;
            Turns = turns;
        }

        // DO NOT ERASE
        public GameInit() { }
    }
}
