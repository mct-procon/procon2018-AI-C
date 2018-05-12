using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.Rule
{
    public class MovableChecker
    {
        public MovableResult MovableCheck<T>(T MeField, T EnemyField, Boards.Player Me, Boards.Player Enemy ) where T : Boards.ColoredBoard
        {
            MovableResult result = new MovableResult();

            uint width = MeField.Width, height = MeField.Height;

            if (Me.Agent1.X >= width || Me.Agent1.X >= height)
            {
                result.Me1 = MovableResultType.OutOfField;
                return result;
            }
            if (Me.Agent2.X >= width || Me.Agent2.X >= height)
            {
                result.Me2 = MovableResultType.OutOfField;
                return result;
            }

            if(Me.Agent1 == Enemy.Agent1 || Me.Agent1 == Enemy.Agent2)
            {
                result.Me1 = MovableResultType.EnemyIsHere;
                return result;
            }
            if(Me.Agent2 == Enemy.Agent2 || Me.Agent2 == Enemy.Agent2)
            {
                result.Me2 = MovableResultType.EnemyIsHere;
                return result;
            }

            if (EnemyField[Me.Agent1])
                result.Me1 = MovableResultType.EraseNeeded;
            if (EnemyField[Me.Agent2])
                result.Me2 = MovableResultType.EraseNeeded;

            return result;
        }

        
    }
}
