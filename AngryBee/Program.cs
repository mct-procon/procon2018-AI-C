using System;
using MCTProcon29Protocol;
using MCTProcon29Protocol.Methods;

namespace AngryBee
{
    class Program : IIPCClientReader
    {
        static IPCManager manager;
        static bool[] calledFlag;
        static Boards.BoardSetting board;
        static Point Me1, Me2, Enemy1, Enemy2;
        static ColoredBoardSmallBigger MeBoard, EnemyBoard;
        static int MeScore, EnemyScore;

        public Program()
        {
            manager = new IPCManager(this);
            calledFlag = new bool[7];
            for (int i = 0; i < 7; i++) { calledFlag[i] = false; }

        }

        public void OnGameInit(GameInit init)
        {
            calledFlag[0] = true;
            board = new Boards.BoardSetting(init.Board, init.BoardWidth, init.BoardHeight);
            Me1 = new Point(init.MeAgent1.X, init.MeAgent2.Y);
            Me2 = new Point(init.MeAgent2.X, init.MeAgent2.Y);
            Enemy1 = new Point(init.EnemyAgent1.X, init.EnemyAgent1.Y);
            Enemy2 = new Point(init.EnemyAgent2.X, init.EnemyAgent2.Y);
        }

        public void OnTurnStart(TurnStart turn)
        {
            calledFlag[1] = true;
            Me1 = new Point(turn.MeAgent1.X, turn.MeAgent2.Y);
            Me2 = new Point(turn.MeAgent2.X, turn.MeAgent2.Y);
            Enemy1 = new Point(turn.EnemyAgent1.X, turn.EnemyAgent1.Y);
            Enemy2 = new Point(turn.EnemyAgent2.X, turn.EnemyAgent2.Y);

            //TODO: Boads.ColoredBoardSmallBiggerへのキャスト
            MeBoard = turn.MeColoredBoard;
            EnemyBoard = turn.EnemyColoredBoard;
        }

        public void OnTurnEnd(TurnEnd turn)
        {
            calledFlag[2] = true;
        }

        public void OnGameEnd(GameEnd end)
        {
            calledFlag[3] = true;
            MeScore = end.MeScore;
            EnemyScore = end.EnemyScore;
        }

        public void OnPause(Pause pause)
        {
            calledFlag[4] = true;
        }

        public void OnInterrupt(Interrupt interrupt)
        {
            calledFlag[5] = true;
        }

        public void OnRebaseByUser(RebaseByUser rebase)
        {
            calledFlag[6] = true;
        }

        //合法手と仮定して考える感じで (Me…移動したプレイヤー)
        public static void UpdateField(ref ColoredBoardSmallBigger MeBoard, ref ColoredBoardSmallBigger EnemyBoard, Boards.Player Me, Boards.Player Enemy)
        {
            if (EnemyBoard[Me.Agent1]) { EnemyBoard[Me.Agent1] = false; }
            else { MeBoard[Me.Agent1] = true; }
            if (EnemyBoard[Me.Agent2]) { EnemyBoard[Me.Agent2] = false; }
            else { MeBoard[Me.Agent2] = true; }
        }

        static void Main(string[] args)
        {
            byte width = 12;
            byte height = 12;

            var com1 = new AI.AI();
            var com2 = new AI.AI();
            var game = Boards.BoardSetting.Generate(height, width);

            var meBoard = new ColoredBoardSmallBigger(height, width);
            var enemyBoard = new ColoredBoardSmallBigger(height, width);

            meBoard[game.me.Agent1] = true;
            meBoard[game.me.Agent2] = true;

            enemyBoard[game.enemy.Agent1] = true;
            enemyBoard[game.enemy.Agent2] = true;

            Console.WriteLine("ターン数を入力＞");
            int maxTurn = int.Parse(Console.ReadLine());
            for (int turn = 0; turn < maxTurn; turn++)
            {
                //初期化
                com1.ends = 0;
                com2.ends = 0;
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                //計算 (深さ = ターン数 * 2)
                Tuple<int, Decided> res = null;
                if (turn % 2 == 0)
                {
                    res = com1.Begin(4, game.setting, meBoard, enemyBoard, game.me, game.enemy);
                    UpdateField(ref meBoard, ref enemyBoard, new Boards.Player(res.Item2.MeAgent1, res.Item2.MeAgent2), game.enemy);
                    game.me = new Boards.Player(res.Item2.MeAgent1, res.Item2.MeAgent2);
                }
                else
                {
                    res = com2.Begin(4, game.setting, enemyBoard, meBoard, game.enemy, game.me);
                    UpdateField(ref enemyBoard, ref meBoard, new Boards.Player(res.Item2.MeAgent1, res.Item2.MeAgent2), game.me);
                    game.enemy = new Boards.Player(res.Item2.MeAgent1, res.Item2.MeAgent2);
                }
                sw.Stop();

                //表示
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(((turn % 2 == 0) ? "先" : "後") + "手番");
                Console.WriteLine("着手(Y1, X1):" + res.Item2.MeAgent1.Y + ", " + res.Item2.MeAgent1.X);
                Console.WriteLine("着手(Y2, X2):" + res.Item2.MeAgent2.Y + ", " + res.Item2.MeAgent2.X);
                for (int i = 0; i < game.setting.ScoreBoard.GetLength(0); ++i)
                {
                    for (int m = 0; m < game.setting.ScoreBoard.GetLength(1); ++m)
                    {
                        string strr = game.setting.ScoreBoard[m, i].ToString();
                        int hoge = 4 - strr.Length;

                        if (meBoard[(uint)m, (uint)i])
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        else if (enemyBoard[(uint)m, (uint)i])
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        else
                            Console.BackgroundColor = ConsoleColor.Black;

                        Point point = new Point((uint)m, (uint)i);
                        if (game.me.Agent1 == point) { Console.BackgroundColor = ConsoleColor.Red; }
                        if (game.me.Agent2 == point) { Console.BackgroundColor = ConsoleColor.Red; }
                        if (game.enemy.Agent1 == point) { Console.BackgroundColor = ConsoleColor.Blue; }
                        if (game.enemy.Agent2 == point) { Console.BackgroundColor = ConsoleColor.Blue; }

                        for (int n = 0; n < hoge; ++n)
                            Console.Write(" ");
                        Console.Write(strr);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();

                Console.WriteLine("End Nodes:{0}[nodes]", (turn % 2 == 0) ? com1.ends : com2.ends);
                Console.WriteLine("Time Elasped:{0}[ms]", sw.ElapsedMilliseconds);

                PointEvaluator.Normal calcPoint = new PointEvaluator.Normal();
                Console.WriteLine("先手スコア = " + calcPoint.Calculate(game.setting.ScoreBoard, meBoard, 0).ToString());
                Console.WriteLine("後手スコア = " + calcPoint.Calculate(game.setting.ScoreBoard, enemyBoard, 0).ToString());
                Console.WriteLine();
            }
        }
    }
}
