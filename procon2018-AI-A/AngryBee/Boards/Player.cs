using System;
using System.Collections.Generic;
using System.Text;
using MCTProcon29Protocol;

namespace AngryBee.Boards
{
    public struct Player
    {
        public Point Agent1 { get; set; }
        public Point Agent2 { get; set; }

        public Player(Point one, Point two)
        {
            Agent1 = one;
            Agent2 = two;
        }
    }
}
