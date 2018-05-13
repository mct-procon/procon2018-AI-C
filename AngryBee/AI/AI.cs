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

        public int ends = 0;

        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> Begin(int deepness, BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            (int DestX, int DestY)[] WayEnumerator = { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };
            return Mini(deepness, WayEnumerator, MeBoard, EnemyBoard, Me, Enemy, int.MaxValue, setting.ScoreBoard);
        }
    
        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> Max(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard,in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int beta, in sbyte[,] ScoreBoard)
        {
            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> result = Tuple.Create( int.MinValue , new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger());
            deepness--;
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                    if (!movable.IsMovable) continue;

                    Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> cache = null;
                    var newMeBoard = MeBoard;

                    if (movable.IsEraseNeeded)
                    {
                        var newEnBoard = EnemyBoard;

                        if (movable.Me1 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent1] = false;
                            newMe.Agent1 = Me.Agent1;
                        }
                        else
                            newMeBoard[newMe.Agent1] = true;

                        if (movable.Me2 == Rule.MovableResultType.EraseNeeded)
                        {
                            newEnBoard[newMe.Agent2] = false;
                            newMe.Agent2 = Me.Agent2;
                        }
                        else
                            newMeBoard[newMe.Agent2] = true;

                        cache = Mini(deepness, WayEnumerator, newMeBoard, newEnBoard, newMe, Enemy, result.Item1, ScoreBoard);
                    } else
                    {
                        newMeBoard[newMe.Agent1] = true;
                        newMeBoard[newMe.Agent2] = true;
                        cache = Mini(deepness, WayEnumerator, newMeBoard, EnemyBoard, newMe, Enemy, result.Item1, ScoreBoard);
                    }

                    if (result.Item1 < cache.Item1)
                        result = cache;
                    if (result.Item1 > beta)
                        return result;
                }

            return result;
        }

        public Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> Mini(int deepness, in (int DestX, int DestY)[] WayEnumerator, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, in sbyte[,] ScoreBoard)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(ScoreBoard, MeBoard, 0), MeBoard, EnemyBoard);
            }

            Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> result = Tuple.Create(int.MaxValue, new ColoredBoardSmallBigger(), new ColoredBoardSmallBigger());
            for (int i = 0; i < WayEnumerator.Length; ++i)
                for (int m = 0; m < WayEnumerator.Length; ++m)
                {
                    Player newEnemy = Enemy;
                    newEnemy.Agent1 += WayEnumerator[i];
                    newEnemy.Agent2 += WayEnumerator[m];

                    var movable = Checker.MovableCheck(EnemyBoard, MeBoard, newEnemy, Me);

                    if (!movable.IsMovable) continue;


                    Tuple<int, ColoredBoardSmallBigger, ColoredBoardSmallBigger> cache = null;
                    var newEnBoard = EnemyBoard;

                    if (movable.IsEraseNeeded)
                    {
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


                        cache = Max(deepness, WayEnumerator, newMeBoard, newEnBoard, Me, newEnemy, result.Item1, ScoreBoard);
                    }
                    else
                    {
                        newEnBoard[newEnemy.Agent1] = true;
                        newEnBoard[newEnemy.Agent2] = true;
                        cache = Max(deepness, WayEnumerator, MeBoard, newEnBoard, Me, newEnemy, result.Item1, ScoreBoard);
                    }

                    if (result.Item1 > cache.Item1)
                        result = cache;
                    if (result.Item1 < alpha)
                        return result;
                }

            return result;
        }
    }
}
