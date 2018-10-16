using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCTProcon29Protocol.Methods
{
    [MessagePackObject]
    public class Connect
    {
        [Key(0)]
        public byte Kind { get; set; }

        public ProgramKind GetProgramKind() => (ProgramKind)Kind;

        [Key(1)]
        public int ProcessId { get; set; }

        public Connect(ProgramKind kind)
        {
            Kind = (byte)kind;
            ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        // DO NOT ERASE
        public Connect(){}
    }


    public enum ProgramKind : byte
    {
        AI = 0, Interface = 1
    }
}
