using AngryBee.Boards;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryBee.AI
{
    public class AI
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        (int DestX, int DestY)[] WayEnumerator = { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

        public int ends = 0;
    
        public int Max(int deepness, BoardSetting setting,in ColoredBoardSmallBigger MeBoard,in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            int result = int.MinValue;
            deepness--;
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                    if (!movable.IsMovable) continue;

                    var newEnBoard = EnemyBoard;
                    var newMeBoard = MeBoard;

                    if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                    {
                        newEnBoard[newMe.Agent1] = false;
                        newMe.Agent1 = Me.Agent1;
                    }
                    else
                        newMeBoard[newMe.Agent1] = true;

                    if(movable.Me2 == Rule.MovableResultType.EraseNeeded)
                    {
                        newEnBoard[newMe.Agent2] = false;
                        newMe.Agent2 = Me.Agent2;
                    }
                    else
                        newMeBoard[newMe.Agent2] = true;

                    result = Math.Max(Mini(deepness, setting, newMeBoard, newEnBoard, newMe, Enemy), result);
                }

            return result;
        }

        public int Mini(int deepness, BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            if (deepness == 0)
                return PointEvaluator.Calculate(setting.ScoreBoard, MeBoard, 0);

            int result = int.MaxValue;
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    Player newEnemy = Enemy;
                    newEnemy.Agent1 += WayEnumerator[i];
                    newEnemy.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(EnemyBoard, MeBoard, newEnemy, Me);

                    if (!movable.IsMovable) continue;

                    var newEnBoard = EnemyBoard;
                    var newMeBoard = MeBoard;

                    if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                    {
                        newMeBoard[newEnemy.Agent1] = false;
                        newEnemy.Agent1 = Enemy.Agent1;
                    }
                    else
                        newEnBoard[newEnemy.Agent1] = true;

                    if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                    {
                        newMeBoard[newEnemy.Agent2] = false;
                        newEnemy.Agent2 = Enemy.Agent2;
                    }
                    else
                        newEnBoard[newEnemy.Agent2] = true;

                    result = Math.Min(Max(deepness, setting, newMeBoard, newEnBoard, Me, newEnemy), result);
                }

            return result;
        }
    }
}
