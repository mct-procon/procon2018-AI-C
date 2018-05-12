using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace AngryBee.Boards
{
    /// <summary>
    /// x-y平面座標での場所を示す構造体
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// x座標（ushort分しか使わない）
        /// </summary>
        public uint X { get; set; }

        /// <summary>
        /// y座標（ushort分しか使わない）
        /// </summary>
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

        public static Point operator +(Point x, (int x,int y) y)
        {
            x.X = (uint)((int)x.X + y.x);
            x.Y = (uint)((int)x.Y + y.y);
            return x;
        }
    }
}
