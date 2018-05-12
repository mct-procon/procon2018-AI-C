using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Boards
{
    public class Player
    {
        public Point Agent1 { get; private set; }
        public Point Agent2 { get; private set; }

        public Player(Point one, Point two)
        {
            Agent1 = one;
            Agent2 = two;
        }
    }
}
