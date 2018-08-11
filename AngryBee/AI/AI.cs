using AngryBee.Boards;
using System;
using System.Collections.Generic;
using System.Text;
using MCTProcon29Protocol.Methods;
using MCTProcon29Protocol;

namespace AngryBee.AI
{
    public class AI
    {
        Rule.MovableChecker Checker = new Rule.MovableChecker();
        PointEvaluator.Normal PointEvaluator = new PointEvaluator.Normal();

        (int DestX, int DestY)[] WayEnumerator = { (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1) };

        public int ends = 0;

        //1ターン = 深さ2
        public Tuple<int, Decided> Begin(int deepness, BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy)
        {
            return Max(deepness, setting, MeBoard, EnemyBoard, Me, Enemy, -int.MaxValue, int.MaxValue);
        }

        //Meが動く
        public Tuple<int, Decided> Max(int deepness, BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(setting.ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(setting.ScoreBoard, EnemyBoard, 0), new Decided());
            }

            Tuple<int, Decided> result = Tuple.Create(int.MinValue, new Decided());

            List<Player> nextMe = new List<Player>();
            Player Killer = new Player(new Point(114, 114), new Point(114, 114));
            MoveOrderling(setting, MeBoard, EnemyBoard, Me, Enemy, Killer, ref nextMe);

            for (int i = 0; i < nextMe.Count; i++)
            {
                Player newMe = Me;
                newMe.Agent1 = nextMe[i].Agent1;
                newMe.Agent2 = nextMe[i].Agent2;

                var movable = Checker.MovableCheck(MeBoard, EnemyBoard, newMe, Enemy);

                if (!movable.IsMovable) continue;

                Tuple<int, Decided> cache = null;
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

                    cache = Mini(deepness - 1, setting, newMeBoard, newEnBoard, newMe, Enemy, Math.Max(result.Item1, alpha), beta);
                }
                else
                {
                    newMeBoard[newMe.Agent1] = true;
                    newMeBoard[newMe.Agent2] = true;
                    cache = Mini(deepness - 1, setting, newMeBoard, EnemyBoard, newMe, Enemy, Math.Max(result.Item1, alpha), beta);
                }

                if (result.Item1 < cache.Item1)
                    result = cache;
                if (result.Item1 >= beta)
                    return result;
            }

            return result;
        }

        //Enemyが動く
        public Tuple<int, Decided> Mini(int deepness, BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, int alpha, int beta)
        {
            if (deepness == 0)
            {
                ends++;
                return Tuple.Create(PointEvaluator.Calculate(setting.ScoreBoard, MeBoard, 0) - PointEvaluator.Calculate(setting.ScoreBoard, EnemyBoard, 0), new Decided());
            }

            Tuple<int, Decided> result = Tuple.Create(int.MaxValue, new Decided());

            List<Player> nextEnemy = new List<Player>();
            Player Killer = new Player(new Point(114, 114), new Point(114, 114));
            MoveOrderling(setting, EnemyBoard, MeBoard, Enemy, Me, Killer, ref nextEnemy);

            for (int i = 0; i < nextEnemy.Count; i++)
            {
                Player newEnemy = Enemy;
                newEnemy.Agent1 = nextEnemy[i].Agent1;
                newEnemy.Agent2 = nextEnemy[i].Agent2;

                var movable = Checker.MovableCheck(EnemyBoard, MeBoard, newEnemy, Me);

                if (!movable.IsMovable) continue;


                Tuple<int, Decided> cache = null;
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


                    cache = Max(deepness - 1, setting, newMeBoard, newEnBoard, Me, newEnemy, alpha, Math.Min(result.Item1, beta));
                }
                else
                {
                    newEnBoard[newEnemy.Agent1] = true;
                    newEnBoard[newEnemy.Agent2] = true;
                    cache = Max(deepness - 1, setting, MeBoard, newEnBoard, Me, newEnemy, alpha, Math.Min(result.Item1, beta));
                }

                if (result.Item1 > cache.Item1)
                    result = cache;
                if (result.Item1 <= alpha)
                    return result;
            }

            return result;
        }

        //遷移順を決める.  「この関数においては」MeBoard…手番プレイヤのボード, Me…手番プレイヤ、とします。
        //(この関数におけるMeは、Maxi関数におけるMe, Mini関数におけるEnemyです）
        //newMe[0]が最初に探索したい行き先、nextMe[1]が次に探索したい行き先…として、nextMeに「次の行き先」を入れていきます。
        //以下のルールで優先順を決めます。
        //ルール1. Killer手があれば、それを優先する。(Killer手がなければ、Killer.Agent1 = (514, 514), Killer.Agent2 = (514, 514)のように範囲外の移動先を設定すること。)
        //ルール2. 次のmoveで得られる「タイルポイント」の合計値、が大きい移動(の組み合わせ)を優先する。
        //なお、ルールはMovableChecker.csに準ずるため、現在は、「タイル除去先にもう一方のエージェントが移動することはできない」として計算しています。
        private void MoveOrderling(in BoardSetting setting, in ColoredBoardSmallBigger MeBoard, in ColoredBoardSmallBigger EnemyBoard, in Player Me, in Player Enemy, in Player Killer, ref List<Player> nextMe)
        {
            uint width = MeBoard.Width;
            uint height = MeBoard.Height;
            List<KeyValuePair<int, Player>> orderling = new List<KeyValuePair<int, Player>>();

            for (int i = 0; i < WayEnumerator.Length; i++)
            {
                for (int m = 0; m < WayEnumerator.Length; m++)
                {
                    Player newMe = Me;
                    newMe.Agent1 += WayEnumerator[i];
                    newMe.Agent2 += WayEnumerator[m];

                    int score = 0;  //優先度 (小さいほど優先度が高い）
                    if (newMe.Agent1 == Killer.Agent1 && newMe.Agent2 == Killer.Agent2) score = -100;
                    else if (newMe.Agent1.X >= width || newMe.Agent1.Y >= height) score = 100;
                    else if (newMe.Agent2.X >= width || newMe.Agent2.Y >= height) score = 100;
                    else if (newMe.Agent1 == newMe.Agent2) score = 100;
                    else if (newMe.Agent1 == Enemy.Agent1) score = 100;
                    else if (newMe.Agent1 == Enemy.Agent2) score = 100;
                    else if (newMe.Agent2 == Enemy.Agent1) score = 100;
                    else if (newMe.Agent2 == Enemy.Agent2) score = 100;
                    else
                    {
                        if (!MeBoard[newMe.Agent1.X, newMe.Agent1.Y] && !EnemyBoard[newMe.Agent1.X, newMe.Agent1.Y])
                        {
                            score += setting.ScoreBoard[newMe.Agent1.X, newMe.Agent1.Y];
                        }
                        if (!MeBoard[newMe.Agent2.X, newMe.Agent2.Y] && !EnemyBoard[newMe.Agent2.X, newMe.Agent2.Y])
                        {
                            score += setting.ScoreBoard[newMe.Agent2.X, newMe.Agent2.Y];
                        }
                        score *= -1;
                    }
                    orderling.Add(new KeyValuePair<int, Player>(score, newMe));
                }
            }
            orderling.Sort((a, b) => a.Key - b.Key);

            for (int i = 0; i < orderling.Count; i++)
            {
                nextMe.Add(orderling[i].Value);
            }
        }
    }
}
