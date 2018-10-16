using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class Pause
    {
        [Key(0)]
        public bool IsEnter { get; set; }

        public Pause(bool isEnter)
        {
            IsEnter = isEnter;
        }

        // DO NOT ERASE
        public Pause() { }
    }
}
