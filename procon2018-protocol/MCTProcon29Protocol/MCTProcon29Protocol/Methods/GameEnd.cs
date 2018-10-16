using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class GameEnd
    {
        [Key(0)]
        public int MeScore { get; set; }

        [Key(1)]
        public int EnemyScore { get; set; }

        public GameEnd(int score, int enemyScore)
        {
            MeScore = score;
            EnemyScore = enemyScore;
        }

        // DO NOT ERASE
        public GameEnd() { }
    }
}
