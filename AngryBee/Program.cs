﻿using System;
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

            Console.WriteLine("[IPC] GameInit Received.");
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

            Console.WriteLine("[IPC] TurnStart Received. Turn is " + turn.Turn.ToString());
        }

        public void OnTurnEnd(TurnEnd turn)
        {
            calledFlag[2] = true;

            Console.WriteLine("[IPC] TurnEnd Received.");
        }

        public void OnGameEnd(GameEnd end)
        {
            calledFlag[3] = true;
            MeScore = end.MeScore;
            EnemyScore = end.EnemyScore;
            Console.WriteLine("[IPC] GameEnd Received.");
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

        static void Main(string[] args)
        {
            Program program = new Program();
            var ai = new AI.AI();
            int portId, maxDepth;

            Console.WriteLine("ポート番号を入力（先手15000, 後手15001)＞");
            portId = int.Parse(Console.ReadLine());
            Console.WriteLine("探索の深さの上限を入力（深さ = ターン数 * 2, 5以下が目安）");
            maxDepth = int.Parse(Console.ReadLine());

            manager.Start(portId);

            {
                var proc = System.Diagnostics.Process.GetCurrentProcess();
                manager.Write(DataKind.Connect, new Connect(ProgramKind.AI) { ProcessId = proc.Id });
                proc.Dispose();
            }
            Console.WriteLine("Sended Connect Method.");

            Console.CancelKeyPress +=
                (o, e) =>
                {
                    manager.ShutdownServer();
                    System.Threading.Thread.Sleep(1000);
                    Environment.Exit(0);
                };

            while (true)
            {
                int i;
                for (i = 0; i < 7; i++) { if (calledFlag[i]) { break; } } 
                if (i == 1)
                {
                    Console.WriteLine($"[AI] Thinking Start.");
                    //TODO: ai.Beginの戻り値を「指し手」にする。
                    var res = ai.Begin(maxDepth, board, MeBoard, EnemyBoard, new Boards.Player(Me1, Me2), new Boards.Player(Enemy1, Enemy2));
                    manager.Write(DataKind.Decided, res.Item2);
                    Console.WriteLine($"[AI] Thinking End, Decided sended.");
                }
                if (i == 3) { break; }
                if(i != 7)
                    calledFlag[i] = false;
            }
            
            /*byte width = 12;
            byte height = 12;

            var ai = new AI.AI();
            var game = Boards.BoardSetting.Generate(height, width);

            var meBoard = new Boards.ColoredBoardSmallBigger(height, width);
            var enemyBoard = new Boards.ColoredBoardSmallBigger(height, width);

            meBoard[game.me.Agent1] = true;
            meBoard[game.me.Agent2] = true;

            enemyBoard[game.enemy.Agent1] = true;
            enemyBoard[game.enemy.Agent2] = true;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var res = ai.Begin(3, game.setting, meBoard, enemyBoard, game.me, game.enemy);

            sw.Stop();
			Console.ForegroundColor = ConsoleColor.White;

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

                    for (int n = 0; n < hoge; ++n)
                        Console.Write(" ");
                    Console.Write(strr);
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            //Console.WriteLine(res);

            Console.WriteLine(res.Item1);

            for (int i = 0; i < game.setting.ScoreBoard.GetLength(1); ++i)
            {
                for (int m = 0; m < game.setting.ScoreBoard.GetLength(0); ++m)
                {
                    string strr = game.setting.ScoreBoard[m, i].ToString();
                    int hoge = 4 - strr.Length;

                    if (res.Item2[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else if (res.Item3[(uint)m, (uint)i])
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    for (int n = 0; n < hoge; ++n)
                        Console.Write(" ");
                    Console.Write(strr);
                }
                Console.WriteLine();
            }

            Console.WriteLine("End Nodes:{0}[nodes]", ai.ends);
            Console.WriteLine("Time Elasped:{0}[ms]", sw.ElapsedMilliseconds);*/
        }
    }
}
