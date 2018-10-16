using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MessagePack;

namespace MCTProcon29Protocol
{
    public interface ColoredBoard
    {
        uint Width { get; }
        uint Height { get; }

        bool this[uint x, uint y] { get; set; }
        bool this[Point p] { get; set; }
    }

    //public struct ColoredBoardSmall : ColoredBoard
    //{
    //    private ulong board;

    //    public uint Width { get; private set; }
    //    public uint Height { get; private set; }

    //    public ColoredBoardSmall(uint width = 0, uint height = 0)
    //    {
    //        if (width * height > 64 || (width == 0 && height == 0))
    //            throw new ArgumentException("x and y are bad numbers.");
    //        Width = width;
    //        Height = height;

    //        board = 0b0;
    //    }

    //    public bool this[uint x, uint y] {
    //        get {
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
    //            return (board & (1ul << (int)(x + (y * Width)))) != 0;
    //        }
    //        set {
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
    //            if (value)
    //                board |= (1ul << (int)(x + (y * Width)));
    //            else
    //                board &= ~(1ul << (int)(x + (y * Width)));
    //        }
    //    }

    //    public bool this[Point p] {
    //        get => this[p.X, p.Y];
    //        set => this[p.X, p.Y] = value;
    //    }
    //}

    [MessagePackObject]
    public unsafe struct ColoredBoardSmallBigger : ColoredBoard
    {
        [IgnoreMember]
        internal const int BoardSize = 12;

        [Key(2)]
        // DO2N'T USE!
        internal fixed ushort board[BoardSize];

        [Key(0)]
        public uint Width { get; private set; }
        [Key(1)]
        public uint Height { get; private set; }

        public ColoredBoardSmallBigger(uint width = 1, uint height = 1)
        {
            if (width > 16 || height > BoardSize || (width == 0 && height == 0))
                throw new ArgumentException("x and y are bad numbers.");
            Width = width;
            Height = height;

            fixed (ushort* ptr = board)
            {
                ushort* itr = ptr;
                for (int i = 0; i < BoardSize; ++i, ++itr)
                {
                    *itr = 0;
                }
            }
        }

        [IgnoreMember]
        public bool this[uint x, uint y] {
            get {
#if DEBUG
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
#endif

                bool result = false;
                fixed (ushort* ptr = board)
                {
                    result = (*(ptr + y) & (1ul << (int)x)) != 0;
                }
                return result;
            }
            set {
#if DEBUG
                if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
#endif
                fixed (ushort* ptr = board)
                {
                    if (value)
                        *(ptr + y) |= (ushort)(1u << (int)x);
                    else
                        *(ptr + y) &= (ushort)(~(1u << (int)x));
                }
            }
        }

        [IgnoreMember]
        public bool this[Point p] {
            get => this[p.X, p.Y];
            set => this[p.X, p.Y] = value;
        }
    }

    //public unsafe struct ColoredBoardNormalSmaller : ColoredBoard
    //{
    //    private const int BoardSize = 32;

    //    private fixed uint board[BoardSize];

    //    public uint Width { get; private set; }
    //    public uint Height { get; private set; }

    //    public ColoredBoardNormalSmaller(uint width = 0, uint height = 0)
    //    {
    //        if (width > 32 || height > 32 || (width == 0 && height == 0))
    //            throw new ArgumentException("x and y are bad numbers.");
    //        Width = width;
    //        Height = height;

    //        fixed (uint* ptr = board)
    //        {
    //            uint* itr = ptr;
    //            for (int i = 0; i < BoardSize; ++i, ++itr)
    //            {
    //                *itr = 0;
    //            }
    //        }
    //    }

    //    public bool this[uint x, uint y] {
    //        get {
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            bool result = false;
    //            fixed (uint* ptr = board)
    //            {
    //                result = (*(ptr + y) & (1ul << (int)x)) != 0;
    //            }
    //            return result;
    //        }
    //        set {
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            fixed (uint* ptr = board)
    //            {
    //                if (value)
    //                    *(ptr + y) |= (1u << (int)x);
    //                else
    //                    *(ptr + y) &= ~(1u << (int)x);
    //            }
    //        }
    //    }

    //    public bool this[Point p] {
    //        get => this[p.X, p.Y];
    //        set => this[p.X, p.Y] = value;
    //    }
    //}

    //public unsafe struct ColoredBoardNormal : ColoredBoard
    //{
    //    private const int BoardSize = 64;

    //    private fixed ulong board[BoardSize];

    //    public uint Width { get; private set; }
    //    public uint Height { get; private set; }

    //    public ColoredBoardNormal(uint width = 0, uint height = 0)
    //    {
    //        if (width > 64 || height > 64 || (width == 0 && height == 0))
    //            throw new ArgumentException("x and y are bad numbers.");
    //        Width = width;
    //        Height = height;

    //        fixed (ulong* ptr = board)
    //        {
    //            ulong* itr = ptr;
    //            for (int i = 0; i < BoardSize; ++i, ++itr)
    //            {
    //                *itr = 0;
    //            }
    //        }
    //    }

    //    public bool this[uint x, uint y] {
    //        get {
    //            if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            bool result = false;
    //            fixed (ulong* ptr = board)
    //            {
    //                result = (*(ptr + y) & (1ul << (int)x)) != 0;
    //            }
    //            return result;
    //        }
    //        set {
    //            if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            fixed (ulong* ptr = board)
    //            {
    //                if (value)
    //                    *(ptr + y) |= (1ul << (int)x);
    //                else
    //                    *(ptr + y) &= ~(1ul << (int)x);
    //            }
    //        }
    //    }

    //    public bool this[Point p] {
    //        get => this[p.X, p.Y];
    //        set => this[p.X, p.Y] = value;
    //    }
    //}

    //public struct ColoredBoardBig : ColoredBoard
    //{

    //    private BigInteger[] board;

    //    public uint Width { get; private set; }
    //    public uint Height { get; private set; }

    //    public ColoredBoardBig(uint width = 0, uint height = 0)
    //    {
    //        if (width == 0 && height == 0)
    //            throw new ArgumentException("x and y are bad numbers.");
    //        Width = width;
    //        Height = height;

    //        board = new BigInteger[Height];

    //        for (uint i = 0; i < Height; ++i)
    //            board[i] = new BigInteger();
    //    }

    //    public bool this[uint x, uint y] {
    //        get {
    //            if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            return (board[y] & (new BigInteger(1) << (int)x)) != 0;
    //        }
    //        set {
    //            if (x < 0 || y < 0) throw new ArgumentException("x and y must be positive numbers.");
    //            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();

    //            if (value)
    //                board[y] |= (new BigInteger(1) << (int)x);
    //            else
    //                board[y] &= ~(new BigInteger(1) << (int)x);
    //        }
    //    }

    //    public bool this[Point p] {
    //        get => this[p.X, p.Y];
    //        set => this[p.X, p.Y] = value;
    //    }
    //}
}
