using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class RebaseByUser
    {
        [Key(0)]
        public Point Agent1 { get; set; }
        [Key(1)]
        public Point Agent2 { get; set; }

        public RebaseByUser(Point agent1, Point agent2)
        {
            Agent1 = agent1;
            Agent2 = agent2;
        }

        // DO NOT ERASE
        public RebaseByUser() { }
    }
}
