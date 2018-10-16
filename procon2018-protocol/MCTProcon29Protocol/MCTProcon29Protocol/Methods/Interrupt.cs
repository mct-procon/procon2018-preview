using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    public class Interrupt
    {
        [Key(0)]
        public bool IsError { get; set; } = false;

        public Interrupt() { }
        public Interrupt(bool isError) => IsError = isError;
    }
}
