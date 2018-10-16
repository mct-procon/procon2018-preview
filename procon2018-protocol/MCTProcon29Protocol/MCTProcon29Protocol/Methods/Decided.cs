using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class Decided
    {
        [Key(0)]
        public VelocityPoint MeAgent1 { get; set; }

        [Key(1)]
        public VelocityPoint MeAgent2 { get; set; }


        public Decided(VelocityPoint agent1, VelocityPoint agent2)
        {
            MeAgent1 = agent1;
            MeAgent2 = agent2;
        }

        // DO NOT ERASE
        public Decided() { }
    }
}
