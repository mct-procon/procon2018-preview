using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    public enum DataKind : byte
    {
        Connect = 0,
        GameInit = 1,
        TurnStart = 2,
        Decided = 3,
        TurnEnd = 4,
        Pause = 5,
        GameEnd = 6,
        Interrupt = 7,
        Resume = 8,
        RebaseByUser = 9
    }
}
