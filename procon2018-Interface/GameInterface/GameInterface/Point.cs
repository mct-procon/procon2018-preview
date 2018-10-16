using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class Point : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point() { }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int CompareTo(object other)
        {
            var point = (Point)other;
            if(this.X == point.X && this.Y == point.Y) return 0;
            return 1;
        }

        public int CompareTo(Point other)
            => X == other.X && Y == other.Y ? 0 : 1;

        public override string ToString() => "(" + X.ToString() + "," + Y.ToString() + ")";
    }
}