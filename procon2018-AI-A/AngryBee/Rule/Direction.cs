using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Rule
{
    public enum Direction : byte
    {
        Stay = 0,
        Up = 1,
        UpRight = 2,
        Right = 3,
        BottomRight = 4,
        Bottom = 5,
        BottomLeft = 6,
        Left = 7,
        TopLeft = 8
    }
}
