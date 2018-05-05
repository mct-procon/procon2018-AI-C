using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Boards
{
    /// <summary>
    /// x-y平面座標での場所を示す構造体
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// x座標
        /// </summary>
        public ushort X { get; set; }

        /// <summary>
        /// y座標
        /// </summary>
        public ushort Y { get; set; }

        public Point(ushort x, ushort y)
        {
            X = x; Y = y;
        }
    }
}
