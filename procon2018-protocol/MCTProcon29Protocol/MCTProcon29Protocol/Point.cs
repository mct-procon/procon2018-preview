using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace MCTProcon29Protocol
{
    /// <summary>
    /// x-y平面座標での場所を示す構造体
    /// </summary>
    [MessagePackObject]
    public struct Point
    {
        /// <summary>
        /// x座標（ushort分しか使わない）
        /// </summary>
        [Key(0)]
        public uint X { get; set; }

        /// <summary>
        /// y座標（ushort分しか使わない）
        /// </summary>
        [Key(1)]
        public uint Y { get; set; }

        public Point(uint x, uint y)
        {
            X = x; Y = y;
        }

        public override int GetHashCode()
            => (int)(X << 16) + (int)Y;

        public override bool Equals(object obj)
        {
            if (obj is Point)
                return this == (Point)obj;
            else
                return false;
        }

        public static bool operator ==(Point x, Point y) => x.X == y.X && x.Y == y.Y;
        public static bool operator !=(Point x, Point y) => x.X != y.X || x.Y != y.Y;

        public static Point operator +(Point x, (int x, int y) y)
        {
            x.X = (uint)((int)x.X + y.x);
            x.Y = (uint)((int)x.Y + y.y);
            return x;
        }

        public static Point operator -(Point x, (int x, int y) y)
        {
            x.X = (uint)((int)x.X - y.x);
            x.Y = (uint)((int)x.Y - y.y);
            return x;
        }

        public static Point operator +(Point x, (uint x, uint y) y)
        {
            x.X += y.x;
            x.Y += y.y;
            return x;
        }

        public static Point operator -(Point x, (uint x, uint y) y)
        {
            x.X -= y.x;
            x.Y -= y.y;
            return x;
        }

        public static Point operator +(Point x, Point y)
        {
            x.X += y.X;
            x.Y += y.Y;
            return x;
        }

        public static Point operator -(Point x, Point y)
        {
            x.X -= y.X;
            x.Y -= y.Y;
            return x;
        }

        public static Point operator +(Point x, VelocityPoint y)
        {
            x.X = (uint)((int)x.X + y.X);
            x.Y = (uint)((int)x.Y + y.Y);
            return x;
        }

        public static Point operator -(Point x, VelocityPoint y)
        {
            x.X = (uint)((int)x.X - y.X);
            x.Y = (uint)((int)x.Y - y.Y);
            return x;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
