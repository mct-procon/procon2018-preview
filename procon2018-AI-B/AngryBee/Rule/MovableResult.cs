using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Rule
{
    public struct MovableResult
    {
        public MovableResultType Me1, Me2;

        public bool IsMovable =>
            ((Me1 | Me2) & MovableResultType.NotMovable) == 0;

        public bool IsEraseNeeded =>
            ((Me1 | Me2) & MovableResultType.EraseNeeded) == MovableResultType.EraseNeeded;
    }

    public enum MovableResultType : byte
    {
        Ok = 0,
        OutOfField = 0b1,
        EnemyIsHere = 0b10,
        EraseNeeded = 0b100,
        NotMovable = 0b11
    }
}
