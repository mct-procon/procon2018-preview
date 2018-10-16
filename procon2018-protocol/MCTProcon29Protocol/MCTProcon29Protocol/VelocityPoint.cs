using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace MCTProcon29Protocol
{
    /// <summary>
    /// x-y平面座標での移動量を示す構造体
    /// </summary>
    [MessagePackObject]
    public struct VelocityPoint
    {
        /// <summary>
        /// x座標の移動量
        /// </summary>
        [Key(0)]
        public int X { get; set; }

        /// <summary>
        /// y座標の移動量
        /// </summary>
        [Key(1)]
        public int Y { get; set; }

        public VelocityPoint(int x, int y)
        {
            X = x; Y = y;
        }

        public override int GetHashCode()
            => (int)(X << 16) + (int)Y;

        public override bool Equals(object obj)
        {
            if (obj is VelocityPoint)
                return this == (VelocityPoint)obj;
            else
                return false;
        }

        public static bool operator ==(VelocityPoint x, VelocityPoint y) => x.X == y.X && x.Y == y.Y;
        public static bool operator !=(VelocityPoint x, VelocityPoint y) => x.X != y.X || x.Y != y.Y;

        public static VelocityPoint operator +(VelocityPoint x, (int x, int y) y)
        {
            x.X = x.X + y.x;
            x.Y = x.Y + y.y;
            return x;
        }

        public static VelocityPoint operator -(VelocityPoint x, (int x, int y) y)
        {
            x.X = x.X - y.x;
            x.Y = x.Y - y.y;
            return x;
        }

        public static implicit operator VelocityPoint((int, int) x) => new VelocityPoint(x.Item1, x.Item2);
    }
}
